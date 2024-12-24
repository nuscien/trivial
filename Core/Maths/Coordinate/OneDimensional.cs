// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Coordinate\OneDimensional.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The 1D models.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

using Trivial.Text;

namespace Trivial.Maths;

/// <summary>
/// The generic 1D (line) coordinate point.
/// </summary>
/// <typeparam name="TUnit">The type of unit.</typeparam>
public class Point1D<TUnit> : SingleElement<TUnit>, IEquatable<Point1D<TUnit>>, IEquatable<SingleElement<TUnit>>, IEquatable<TUnit> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
{
    /// <summary>
    /// Initializes a new instance of the OneDimensionalPoint class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public Point1D()
    {
    }

    /// <summary>
    /// Initializes a new instance of the OneDimensionalPoint class.
    /// </summary>
    /// <param name="x">The value of X.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public Point1D(TUnit x)
    {
        X = x;
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
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
        => new Point1D<TUnit>(X);

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(SingleElement<TUnit> other)
        => other != null && X.Equals(other.ItemA);

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(Point1D<TUnit> other)
        => other != null && X.Equals(other.X);

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(TUnit other)
        => X.Equals(other);

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other is Point1D<TUnit> p) return Equals(p);
        if (other is TUnit u) return Equals(u);
        if (other is SingleElement<TUnit> e) return Equals(e);
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
        => X.ToString();
}

/// <summary>
/// The point of 1D (line) mathematics coordinate.
/// </summary>
[JsonConverter(typeof(DoublePoint1DConverter))]
public sealed class DoublePoint1D : Point1D<double>, IAdvancedAdditionCapable<DoublePoint1D>, IEquatable<Point1D<float>>, IEquatable<Point1D<int>>, IEquatable<float>, IEquatable<int>, IComparable<Point1D<double>>, IComparable<Point1D<float>>, IComparable<Point1D<int>>, IComparable<double>, IComparable<float>, IComparable<int>, IJsonObjectHost
#if NET8_0_OR_GREATER
    , IAdditionOperators<DoublePoint1D, DoublePoint1D, DoublePoint1D>, ISubtractionOperators<DoublePoint1D, DoublePoint1D, DoublePoint1D>, IUnaryNegationOperators<DoublePoint1D, DoublePoint1D>, IParsable<DoublePoint1D>
#endif
{
    /// <summary>
    /// Initializes a new instance of the DoublePoint1D class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public DoublePoint1D()
    {
    }

    /// <summary>
    /// Initializes a new instance of the DoublePoint1D class.
    /// </summary>
    /// <param name="x">The value of X.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public DoublePoint1D(double x) : base(x)
    {
    }

    /// <summary>
    /// Gets a value indicating whether the current element is negative.
    /// true if it is positve or zero; otherwise, false.
    /// </summary>
    public bool IsNegative => X < 0;

    /// <summary>
    /// Gets a value indicating whether the current element is a unit element of zero.
    /// </summary>
    public bool IsZero => X == 0;

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public DoublePoint1D Plus(Point1D<double> value)
        => value != null ? new(X + value.X) : new(X);

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public DoublePoint1D Plus(Point1D<float> value)
        => value != null ? new(X + value.X) : new(X);

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public DoublePoint1D Plus(Point1D<int> value)
        => value != null ? new(X + value.X) : new(X);

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public DoublePoint1D Plus(DoublePoint1D value)
        => value != null ? new(X + value.X) : new(X);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public DoublePoint1D Minus(Point1D<double> value)
        => value != null ? new(X - value.X) : new(X);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public DoublePoint1D Minus(Point1D<float> value)
        => value != null ? new(X - value.X) : new(X);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public DoublePoint1D Minus(Point1D<int> value)
        => value != null ? new(X - value.X) : new(X);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public DoublePoint1D Minus(DoublePoint1D value)
        => value != null ? new(X - value.X) : new(X);

    /// <summary>
    /// Negates the current value to return. Current value will not be changed.
    /// -this
    /// </summary>
    /// <returns>A result after negation.</returns>
    public DoublePoint1D Negate()
        => new(-X);

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
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
        => new DoublePoint1D(X);

    /// <summary>
    /// Gets a unit element for addition and subtraction.
    /// 0
    /// </summary>
    /// <returns>An element zero for the value.</returns>
    public DoublePoint1D GetElementZero()
        => new(0);

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
    /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
    /// Zero This object is equal to <paramref name="other"/>.
    /// Greater than zero This object is greater than <paramref name="other"/>. 
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public int CompareTo(Point1D<double> other)
    {
        if (other is null) return X.CompareTo(null);
        return X.CompareTo(other.X);
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
    /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
    /// Zero This object is equal to <paramref name="other"/>.
    /// Greater than zero This object is greater than <paramref name="other"/>. 
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public int CompareTo(Point1D<float> other)
    {
        if (other is null) return X.CompareTo(null);
        return X.CompareTo(other.X);
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
    /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
    /// Zero This object is equal to <paramref name="other"/>.
    /// Greater than zero This object is greater than <paramref name="other"/>. 
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public int CompareTo(Point1D<int> other)
    {
        if (other is null) return X.CompareTo(null);
        return X.CompareTo(other.X);
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
    /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
    /// Zero This object is equal to <paramref name="other"/>.
    /// Greater than zero This object is greater than <paramref name="other"/>. 
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public int CompareTo(double other)
        => X.CompareTo(other);

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
    /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
    /// Zero This object is equal to <paramref name="other"/>.
    /// Greater than zero This object is greater than <paramref name="other"/>. 
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public int CompareTo(float other)
        => X.CompareTo(other);

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
    /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
    /// Zero This object is equal to <paramref name="other"/>.
    /// Greater than zero This object is greater than <paramref name="other"/>. 
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public int CompareTo(int other)
        => X.CompareTo(other);

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(int other)
        => X == other;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(float other)
        => X == other;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(Point1D<int> other)
        => other is not null && X == other.X;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(Point1D<float> other)
        => other is not null && (Math.Abs(X - other.X) < Arithmetic.DoubleAccuracy);

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other is Point1D<double> p) return Equals(p);
        if (other is Point1D<float> p1) return Equals(p1);
        if (other is Point1D<int> p2) return Equals(p2);
        if (other is double n1) return Equals(n1);
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
    /// Parses the specific input string to a point.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <returns>A point.</returns>
    /// <exception cref="FormatException">s was invalid.</exception>
    public static DoublePoint1D Parse(string s)
        => s is null ? null : (TryParse(s) ?? throw new FormatException("s is not valid."));

    /// <summary>
    /// Parses the specific input string to a point.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <returns>A point; or null, if parse failed.</returns>
    public static DoublePoint1D TryParse(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (double.TryParse(s, out var f)) return new(f);
        if (Numbers.TryParseToInt32(s, 10, out var integer)) return new(integer);
        return null;
    }

#if NET8_0_OR_GREATER
    /// <summary>
    /// Parses the specific input string to a point.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information about s.</param>
    /// <returns>A point.</returns>
    /// <exception cref="FormatException">s was invalid.</exception>
    public static DoublePoint1D Parse(string s, IFormatProvider provider)
        => s is null ? null : (TryParse(s, provider, out var r) ? r : throw new FormatException("s is not valid."));

    /// <summary>
    /// Parses the specific input string to a point.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information about s.</param>
    /// <param name="result">When this method returns, contains the result of successfully parsing; or a default value on failure.</param>
    /// <returns>true if parse successfully; otherwise, false.</returns>
    public static bool TryParse(string s, IFormatProvider provider, out DoublePoint1D result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }

        if (double.TryParse(s, provider, out var f))
        {
            result = new(f);
            return true;
        }

        if (Numbers.TryParseToInt32(s, 10, out var integer))
        {
            result = new(integer);
            return true;
        }

        result = default;
        return false;
    }
#endif

    /// <summary>
    /// Converts a number to one dimensional point.
    /// </summary>
    /// <param name="value">The point.</param>
    public static implicit operator DoublePoint1D(double value)
        => new(value);

    /// <summary>
    /// Converts a number to one dimensional point.
    /// </summary>
    /// <param name="value">The point.</param>
    public static implicit operator DoublePoint1D(long value)
        => new(value);

    /// <summary>
    /// Converts a number to one dimensional point.
    /// </summary>
    /// <param name="value">The point.</param>
    public static implicit operator DoublePoint1D(int value)
        => new(value);

    /// <summary>
    /// Pluses two points in coordinate.
    /// </summary>
    /// <param name="leftValue">The left value for addition operator.</param>
    /// <param name="rightValue">The right value for addition operator.</param>
    /// <returns>A result after addition.</returns>
    public static DoublePoint1D operator +(DoublePoint1D leftValue, DoublePoint1D rightValue)
        => (leftValue ?? new()).Plus(rightValue);

    /// <summary>
    /// Pluses two points in coordinate.
    /// </summary>
    /// <param name="leftValue">The left value for addition operator.</param>
    /// <param name="rightValue">The right value for addition operator.</param>
    /// <returns>A result after addition.</returns>
    public static DoublePoint1D operator -(DoublePoint1D leftValue, DoublePoint1D rightValue)
        => (leftValue ?? new()).Minus(rightValue);

    /// <summary>
    /// Negates the current value to return. Current value will not be changed.
    /// </summary>
    /// <param name="value">The value to negate.</param>
    /// <returns>A result after negation.</returns>
    public static DoublePoint1D operator -(DoublePoint1D value)
        => value?.Negate();
}

/// <summary>
/// The point of 1D (line) integer coordinate.
/// </summary>
[JsonConverter(typeof(IntPoint1DConverter))]
public sealed class IntPoint1D : Point1D<int>, IAdvancedAdditionCapable<IntPoint1D>, IComparable<Point1D<double>>, IComparable<Point1D<int>>, IComparable<double>, IComparable<int>, IJsonObjectHost
#if NET8_0_OR_GREATER
    , IAdditionOperators<IntPoint1D, IntPoint1D, IntPoint1D>, ISubtractionOperators<IntPoint1D, IntPoint1D, IntPoint1D>, IUnaryNegationOperators<IntPoint1D, IntPoint1D>, IParsable<IntPoint1D>
#endif
{
    /// <summary>
    /// Initializes a new instance of the Int32Point1D class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public IntPoint1D()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Int32Point1D class.
    /// </summary>
    /// <param name="x">The value of X.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public IntPoint1D(int x) : base(x)
    {
    }

    /// <summary>
    /// Gets a value indicating whether the current element is negative.
    /// true if it is positve or zero; otherwise, false.
    /// </summary>
    public bool IsNegative => X < 0;

    /// <summary>
    /// Gets a value indicating whether the current element is a unit element of zero.
    /// </summary>
    public bool IsZero => X == 0;

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public IntPoint1D Plus(Point1D<int> value)
        => value != null ? new(X + value.X) : new(X);

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public IntPoint1D Plus(IntPoint1D value)
        => value != null ? new(X + value.X) : new(X);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public IntPoint1D Minus(Point1D<int> value)
        => value != null ? new(X - value.X) : new(X);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public IntPoint1D Minus(IntPoint1D value)
        => value != null ? new(X - value.X) : new(X);

    /// <summary>
    /// Negates the current value to return. Current value will not be changed.
    /// -this
    /// </summary>
    /// <returns>A result after negation.</returns>
    public IntPoint1D Negate()
        => new(-X);

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
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
        => new IntPoint1D(X);

    /// <summary>
    /// Gets a unit element for addition and subtraction.
    /// 0
    /// </summary>
    /// <returns>An element zero for the value.</returns>
    IntPoint1D IAdvancedAdditionCapable<IntPoint1D>.GetElementZero()
        => new(0);

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
    /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
    /// Zero This object is equal to <paramref name="other"/>.
    /// Greater than zero This object is greater than <paramref name="other"/>. 
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public int CompareTo(Point1D<double> other)
    {
        if (other is null) return X.CompareTo(null);
        return X.CompareTo(other.X);
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
    /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
    /// Zero This object is equal to <paramref name="other"/>.
    /// Greater than zero This object is greater than <paramref name="other"/>. 
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public int CompareTo(Point1D<int> other)
    {
        if (other is null) return X.CompareTo(null);
        return X.CompareTo(other.X);
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
    /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
    /// Zero This object is equal to <paramref name="other"/>.
    /// Greater than zero This object is greater than <paramref name="other"/>. 
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public int CompareTo(double other)
        => ((double)X).CompareTo(other);

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
    /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
    /// Zero This object is equal to <paramref name="other"/>.
    /// Greater than zero This object is greater than <paramref name="other"/>. 
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public int CompareTo(int other)
        => X.CompareTo(other);

    /// <summary>
    /// Parses the specific input string to a point.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <returns>A point.</returns>
    /// <exception cref="FormatException">s was invalid.</exception>
    public static IntPoint1D Parse(string s)
        => s is null ? null : (TryParse(s) ?? throw new FormatException("s is not valid."));

    /// <summary>
    /// Parses the specific input string to a point.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <returns>A point; or null, if parse failed.</returns>
    public static IntPoint1D TryParse(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (Numbers.TryParseToInt32(s, 10, out var integer)) return new(integer);
        if (double.TryParse(s, out var f)) return new((int)f);
        return null;
    }

#if NET8_0_OR_GREATER
    /// <summary>
    /// Parses the specific input string to a point.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information about s.</param>
    /// <returns>A point.</returns>
    /// <exception cref="FormatException">s was invalid.</exception>
    public static IntPoint1D Parse(string s, IFormatProvider provider)
        => s is null ? null : (TryParse(s) ?? throw new FormatException("s is not valid."));

    /// <summary>
    /// Parses the specific input string to a point.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information about s.</param>
    /// <param name="result">When this method returns, contains the result of successfully parsing; or a default value on failure.</param>
    /// <returns>true if parse successfully; otherwise, false.</returns>
    public static bool TryParse(string s, IFormatProvider provider, out IntPoint1D result)
    {
        result = TryParse(s);
        return result != null;
    }
#endif

    /// <summary>
    /// Converts a number to one dimensional point.
    /// </summary>
    /// <param name="value">The point.</param>
    public static implicit operator IntPoint1D(int value)
        => new(value);

    /// <summary>
    /// Pluses two points in coordinate.
    /// </summary>
    /// <param name="leftValue">The left value for addition operator.</param>
    /// <param name="rightValue">The right value for addition operator.</param>
    /// <returns>A result after addition.</returns>
    public static IntPoint1D operator +(IntPoint1D leftValue, IntPoint1D rightValue)
        => (leftValue ?? new()).Plus(rightValue);

    /// <summary>
    /// Pluses two points in coordinate.
    /// </summary>
    /// <param name="leftValue">The left value for addition operator.</param>
    /// <param name="rightValue">The right value for addition operator.</param>
    /// <returns>A result after addition.</returns>
    public static IntPoint1D operator -(IntPoint1D leftValue, IntPoint1D rightValue)
        => (leftValue ?? new()).Minus(rightValue);

    /// <summary>
    /// Negates the current value to return. Current value will not be changed.
    /// </summary>
    /// <param name="value">The value to negate.</param>
    /// <returns>A result after negation.</returns>
    public static IntPoint1D operator -(IntPoint1D value)
        => value?.Negate();
}

/// <summary>
/// Json point converter.
/// </summary>
sealed class IntPoint1DConverter : JsonConverter<IntPoint1D>
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
    public override IntPoint1D Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            return reader.TokenType switch
            {
                JsonTokenType.Null => null,
                JsonTokenType.Number => reader.TryGetInt32(out var integer) ? new IntPoint1D(integer) : new IntPoint1D((int)reader.GetDouble()),
                JsonTokenType.String => IntPoint1D.Parse(reader.GetString()),
                JsonTokenType.False => null,
                JsonTokenType.StartObject => ReadJson(ref reader),
                JsonTokenType.StartArray => ReadJsonArray(ref reader),
                _ => throw new JsonException($"The token type is {reader.TokenType} but expect a JSON or a number.")
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
    public override void Write(Utf8JsonWriter writer, IntPoint1D value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteNumber("x", value.X);
        writer.WriteEndObject();
    }

    private static IntPoint1D ReadJson(ref Utf8JsonReader reader)
    {
        var json = JsonObjectNode.ParseValue(ref reader) ?? throw new JsonException("Cannot parse the JSON.");
        if (json.TryGetInt32Value("x", out var integer)) return new(integer);
        if (json.TryGetInt32Value("X", out var integer2)) return new(integer2);
        throw new JsonException("Expect a property x in the JSON.");
    }

    private static IntPoint1D ReadJsonArray(ref Utf8JsonReader reader)
    {
        var json = JsonArrayNode.ParseValue(ref reader) ?? throw new JsonException("Cannot parse the JSON.");
        if (json.Count != 1) throw new JsonException("The count of the JSON array is not expected.");
        if (!json.TryGetInt32Value(0, out var num))
            throw new JsonException("The type of the array item is not expected.");
        return new(num);
    }
}

/// <summary>
/// Json point converter.
/// </summary>
sealed class DoublePoint1DConverter : JsonConverter<DoublePoint1D>
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
    public override DoublePoint1D Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            return reader.TokenType switch
            {
                JsonTokenType.Null => null,
                JsonTokenType.Number => reader.TryGetInt32(out var integer) ? new DoublePoint1D(integer) : new DoublePoint1D(reader.GetDouble()),
                JsonTokenType.String => DoublePoint1D.Parse(reader.GetString()),
                JsonTokenType.False => null,
                JsonTokenType.StartObject => ReadJson(ref reader),
                JsonTokenType.StartArray => ReadJsonArray(ref reader),
                _ => throw new JsonException($"The token type is {reader.TokenType} but expect a JSON or a number.")
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
    public override void Write(Utf8JsonWriter writer, DoublePoint1D value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteNumber("x", value.X);
        writer.WriteEndObject();
    }

    private static DoublePoint1D ReadJson(ref Utf8JsonReader reader)
    {
        var json = JsonObjectNode.ParseValue(ref reader) ?? throw new JsonException("Cannot parse the JSON.");
        if (json.TryGetDoubleValue("x", out var f)) return new(f);
        if (json.TryGetDoubleValue("X", out var f2)) return new(f2);
        throw new JsonException("Expect a property x in the JSON.");
    }

    private static DoublePoint1D ReadJsonArray(ref Utf8JsonReader reader)
    {
        var json = JsonArrayNode.ParseValue(ref reader) ?? throw new JsonException("Cannot parse the JSON.");
        if (json.Count != 1) throw new JsonException("The count of the JSON array is not expected.");
        if (!json.TryGetDoubleValue(0, out var num))
            throw new JsonException("The type of the array item is not expected.");
        return new(num);
    }
}
