using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Trivial.Maths;

/// <summary>
/// The line segment in coordinate.
/// </summary>
[DataContract]
public class LineSegment : IPixelOutline<double>, ICoordinateSinglePoint<double>, ICloneable, IEquatable<LineSegment>
{
    private DoublePoint2D start;
    private DoublePoint2D end;

    /// <summary>
    /// Initializes a new instance of the LineSegment class.
    /// </summary>
    public LineSegment()
    {
        start = new();
        end = new();
    }

    /// <summary>
    /// Initializes a new instance of the LineSegment class.
    /// </summary>
    /// <param name="x1">X of the start point.</param>
    /// <param name="y1">Y of the start point.</param>
    /// <param name="x2">X of the end point.</param>
    /// <param name="y2">Y of the end point.</param>
    public LineSegment(double x1, double y1, double x2, double y2)
    {
        start = new(x1, y1);
        end = new(x2, y2);
    }

    /// <summary>
    /// Initializes a new instance of the LineSegment class.
    /// </summary>
    /// <param name="start">The start point.</param>
    /// <param name="end">The end point.</param>
    public LineSegment(DoublePoint2D start, DoublePoint2D end)
    {
        this.start = start ?? new();
        this.end = end ?? new();
    }

    /// <summary>
    /// Initializes a new instance of the LineSegment class.
    /// </summary>
    /// <param name="line">The line segment.</param>
    public LineSegment(LineSegmentF line)
    {
        if (line == null) line = new();
        start = line.Start;
        end = line.End;
    }

    /// <summary>
    /// Gets or sets the start point.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public DoublePoint2D Start
    {
        get => start;
        set => start = value ?? new();
    }

    /// <summary>
    /// Gets or sets the end point.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public DoublePoint2D End
    {
        get => end;
        set => end = value ?? new();
    }

    /// <summary>
    /// Gets the x of start point.
    /// </summary>
    [JsonPropertyName("x1")]
    [DataMember(Name = "x1")]
    public double StartX
    {
        get => Start.X;
        set => Start.X = value;
    }

    /// <summary>
    /// Gets the x of start point.
    /// </summary>
    [JsonPropertyName("y1")]
    [DataMember(Name = "y1")]
    public double StartY
    {
        get => Start.Y;
        set => Start.Y = value;
    }

    /// <summary>
    /// Gets the x of end point.
    /// </summary>
    [JsonPropertyName("x2")]
    [DataMember(Name = "x2")]
    public double EndX
    {
        get => End.X;
        set => End.X = value;
    }

    /// <summary>
    /// Gets the x of end point.
    /// </summary>
    [JsonPropertyName("y2")]
    [DataMember(Name = "y2")]
    public double EndY
    {
        get => End.Y;
        set => End.Y = value;
    }

    /// <summary>
    /// Gets the length.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public double Length => Math.Sqrt(Math.Pow(End.X - Start.X, 2) + Math.Pow(End.Y - Start.Y, 2));

    /// <summary>
    /// Returns a string that represents the line.
    /// </summary>
    /// <returns>A string that represents the line.</returns>
    public override string ToString()
    {
        try
        {
            return $"{StartX:0.########}, {StartY:0.########} → {EndX:0.########}, {EndY:0.########}";
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (FormatException)
        {
        }
        catch (NullReferenceException)
        {
        }

        return $"{StartX}, {StartY} → {EndX}, {EndY}";
    }

    /// <summary>
    /// Gets y by x.
    /// </summary>
    /// <param name="x">X.</param>
    /// <returns>Y.</returns>
    public double GetY(double x)
        => (x - StartX) / (StartX - EndX) * (StartY - EndY) + StartY;

    /// <summary>
    /// Gets x by y.
    /// </summary>
    /// <param name="y">Y.</param>
    /// <returns>X.</returns>
    public double GetX(double y)
        => (y - StartY) / (StartY - EndY) * (StartX - EndX) + StartX;

    /// <summary>
    /// Test if a point is on the line.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>true if the point is on the line; otherwise, false.</returns>
    public bool Contains(Point2D<double> point)
        => point != null && Math.Abs((point.Y - StartY) / (StartY - EndY) - (point.X - StartX) / (StartX - EndX)) < InternalHelper.DoubleAccuracy;

    /// <summary>
    /// Generates point collection in the specific zone and accuracy.
    /// </summary>
    /// <param name="left">The left boundary.</param>
    /// <param name="right">The right boundary.</param>
    /// <param name="accuracy">The step in x.</param>
    /// <returns>A point collection.</returns>
    public IEnumerable<DoublePoint2D> DrawPoints(double left, double right, double accuracy)
        => InternalHelper.DrawPoints(this, left, right, accuracy);

    /// <summary>
    /// Generates point collection in the specific zone and accuracy.
    /// </summary>
    /// <param name="left">The left boundary.</param>
    /// <param name="right">The right boundary.</param>
    /// <param name="accuracy">The step in x.</param>
    /// <returns>A point collection.</returns>
    IEnumerable<Point2D<double>> IPixelOutline<double>.DrawPoints(double left, double right, double accuracy)
        => InternalHelper.DrawPoints(this, left, right, accuracy);

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>An instance copied from current one.</returns>
    public LineSegment Clone()
        => new(start, end);

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>An instance copied from current one.</returns>
    object ICloneable.Clone()
        => Clone();

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(LineSegment other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return StartX == other.StartX && StartY == other.StartY && EndX == other.EndX && EndY == other.EndY;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        return Equals(obj as LineSegment);
    }

    /// <inheritdoc />
    public override int GetHashCode()
        => Tuple.Create(StartX, StartY, EndX, EndY).GetHashCode();

    /// <summary>
    /// Compares two angles to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(LineSegment leftValue, LineSegment rightValue)
    {
        if (leftValue is null && rightValue is null) return true;
        if (leftValue is null || rightValue is null) return false;
        return leftValue.Equals(rightValue);
    }

    /// <summary>
    /// Compares two angles to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(LineSegment leftValue, LineSegment rightValue)
    {
        if (leftValue is null && rightValue is null) return false;
        if (leftValue is null || rightValue is null) return true;
        return !leftValue.Equals(rightValue);
    }

    /// <summary>
    /// Converts to a line.
    /// </summary>
    /// <param name="value">The line segment.</param>
    public static explicit operator StraightLine(LineSegment value)
    {
        if (value is null) return null;
        var width = value.Start.X - value.End.X;
        var height = value.Start.Y - value.End.Y;
        return new StraightLine(width, -height, value.Start.Y / height - value.Start.X / width);
    }
}

/// <summary>
/// The straight line in coordinate.
/// </summary>
[DataContract]
public class StraightLine : IPixelOutline<double>, ICoordinateSinglePoint<double>, IEquatable<StraightLine>
{
    /// <summary>
    /// <para>Initializes a new instance of the StraightLine class.</para>
    /// <para>General form: ax+by+c=0 (a≥0).</para>
    /// </summary>
    public StraightLine()
    {
        A = 1;
        B = -1;
        C = 0;
        Slope = 1;
        Intercept = 0;
    }

    /// <summary>
    /// <para>Initializes a new instance of the StraightLine class.</para>
    /// <para>General form: ax+by+c=0 (a≥0).</para>
    /// </summary>
    /// <param name="a">Parameter a.</param>
    /// <param name="b">Parameter b.</param>
    /// <param name="c">Parameter c.</param>
    public StraightLine(double a, double b, double c)
    {
        A = a;
        B = b;
        C = c;
        Slope = GetSlope(a, b);
        Intercept = b == 0 || double.IsNaN(b) ? double.NaN : -c / b;
    }

    /// <summary>
    /// <para>Initializes a new instance of the StraightLine class.</para>
    /// <para>Slope intercept form: y=kx+b.</para>
    /// </summary>
    /// <param name="k">Parameter k.</param>
    /// <param name="b">Parameter b.</param>
    public StraightLine(double k, double b)
    {
        Slope = k;
        Intercept = b;
        B = -1;
        A = k;
        C = b;
    }

    /// <summary>
    /// <para>Initializes a new instance of the StraightLine class.</para>
    /// </summary>
    /// <param name="line">The line segment to extend.</param>
    public StraightLine(LineSegment line)
        : this(line.Start, line.End)
    {
    }

    /// <summary>
    /// <para>Initializes a new instance of the StraightLine class.</para>
    /// </summary>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    public StraightLine(DoublePoint2D a, DoublePoint2D b)
    {
        if (a == null) a = new();
        if (b == null) b = new();
        if (a.X == b.X && a.Y == b.Y)
        {
            A = 1;
            B = -1;
            C = 0;
            Slope = 1;
            Intercept = 0;
            return;
        }

        var sign = 1;
        A = b.Y - a.Y;
        if (A < 0)
        {
            sign = -1;
            A = sign * A;
        }

        B = sign * (a.X - b.X);
        C = sign * (a.Y * b.X - a.X * b.Y);
        Slope = GetSlope(A, B);
        Intercept = B == 0 || double.IsNaN(B) ? double.NaN : -C / B;
    }

    /// <summary>
    /// <para>Initializes a new instance of the StraightLine class.</para>
    /// </summary>
    /// <param name="line">The line.</param>
    public StraightLine(StraightLineF line)
    {
        if (line == null) line = new();
        Slope = line.Slope;
        Intercept = line.Intercept;
        B = line.B;
        A = line.A;
        C = line.C;
    }

    /// <summary>
    /// Gets or sets parameter a in general form.
    /// </summary>
    [JsonPropertyName("a")]
    [DataMember(Name = "a")]
    public double A { get; }

    /// <summary>
    /// Gets or sets parameter b in general form.
    /// </summary>
    [JsonPropertyName("b")]
    [DataMember(Name = "b")]
    public double B { get; }

    /// <summary>
    /// Gets or sets parameter c in general form.
    /// </summary>
    [JsonPropertyName("c")]
    [DataMember(Name = "c")]
    public double C { get; }

    /// <summary>
    /// Gets or sets the slope. Parameter k in slope intercept form.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public double Slope { get; }

    /// <summary>
    /// Gets or sets the intercept. Parameter b in slope intercept form.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public double Intercept { get; }

    /// <summary>
    /// Gets y by x.
    /// </summary>
    /// <param name="x">X.</param>
    /// <returns>Y.</returns>
    public double GetY(double x)
        => -(A * x + C) / B;

    /// <summary>
    /// Gets x by y.
    /// </summary>
    /// <param name="y">Y.</param>
    /// <returns>X.</returns>
    public double GetX(double y)
        => -(B * y + C) / A;

    /// <summary>
    /// Test if a point is on the line.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>true if the point is on the line; otherwise, false.</returns>
    public bool Contains(Point2D<double> point)
        => point != null && Math.Abs(A * point.X + B * point.Y + C) < InternalHelper.DoubleAccuracy;

    /// <summary>
    /// Generates point collection in the specific zone and accuracy.
    /// </summary>
    /// <param name="left">The left boundary.</param>
    /// <param name="right">The right boundary.</param>
    /// <param name="accuracy">The step in x.</param>
    /// <returns>A point collection.</returns>
    public IEnumerable<DoublePoint2D> DrawPoints(double left, double right, double accuracy)
        => InternalHelper.DrawPoints(this, left, right, accuracy);

    /// <summary>
    /// Generates point collection in the specific zone and accuracy.
    /// </summary>
    /// <param name="left">The left boundary.</param>
    /// <param name="right">The right boundary.</param>
    /// <param name="accuracy">The step in x.</param>
    /// <returns>A point collection.</returns>
    IEnumerable<Point2D<double>> IPixelOutline<double>.DrawPoints(double left, double right, double accuracy)
        => InternalHelper.DrawPoints(this, left, right, accuracy);

    /// <summary>
    /// Returns a string that represents the line.
    /// </summary>
    /// <returns>A string that represents the line.</returns>
    public override string ToString()
    {
        try
        {
            return $"{A:0.########} x + {B:0.########} y + {C:0.########} = 0";
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (FormatException)
        {
        }
        catch (NullReferenceException)
        {
        }

        return $"{A} x + {B} y + {C} = 0";
    }
    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(StraightLine other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return A == other.A && B == other.B && C == other.C;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        return Equals(obj as StraightLine);
    }

    /// <inheritdoc />
    public override int GetHashCode()
        => Tuple.Create(A, B, C).GetHashCode();

    /// <summary>
    /// Compares two angles to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(StraightLine leftValue, StraightLine rightValue)
    {
        if (leftValue is null && rightValue is null) return true;
        if (leftValue is null || rightValue is null) return false;
        return leftValue.Equals(rightValue);
    }

    /// <summary>
    /// Compares two angles to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(StraightLine leftValue, StraightLine rightValue)
    {
        if (leftValue is null && rightValue is null) return false;
        if (leftValue is null || rightValue is null) return true;
        return !leftValue.Equals(rightValue);
    }

    /// <summary>
    /// Converts to angle.
    /// </summary>
    /// <param name="value">The line.</param>
    public static explicit operator Angle(StraightLine value)
        => value is null ? new Angle(0) : Geometry.Angle(value);

    private static double GetSlope(double a, double b)
    {
        if (b == 0 || double.IsNaN(b) || (b <= InternalHelper.DoubleAccuracy && b >= -InternalHelper.DoubleAccuracy))
        {
            if (a > InternalHelper.DoubleAccuracy) return double.PositiveInfinity;
            if (a < -InternalHelper.DoubleAccuracy) return double.NegativeInfinity;
            return double.NaN;
        }

        return -a / b;
    }
}
