using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Trivial.Maths;

/// <summary>
/// The circle in coordinate.
/// </summary>
[DataContract]
public class CoordinateCircleF : IPixelOutline<float>, ICoordinateTuplePoint<float>, ICloneable, IEquatable<CoordinateCircleF>
{
    private PointF center;
    private float radius;

    /// <summary>
    /// Initializes a new instance of the CoordinateCircleF class.
    /// </summary>
    public CoordinateCircleF()
    {
        center = new();
        radius = 0;
    }

    /// <summary>
    /// Initializes a new instance of the CoordinateCircleF class.
    /// </summary>
    /// <param name="r">The radius.</param>
    public CoordinateCircleF(float r)
    {
        center = new(0, 0);
        radius = float.IsNaN(r) ? 0 : Math.Abs(r);
    }

    /// <summary>
    /// Initializes a new instance of the CoordinateCircleF class.
    /// </summary>
    /// <param name="x">The x of center point.</param>
    /// <param name="y">The y of center point.</param>
    /// <param name="r">The radius.</param>
    public CoordinateCircleF(float x, float y, float r)
    {
        center = new(x, y);
        radius = float.IsNaN(r) ? 0 : Math.Abs(r);
    }

    /// <summary>
    /// Initializes a new instance of the CoordinateCircleF class.
    /// </summary>
    /// <param name="center">The center point.</param>
    /// <param name="r">The radius.</param>
    public CoordinateCircleF(PointF center, float r)
    {
        this.center = center;
        radius = float.IsNaN(r) ? 0 : Math.Abs(r);
    }

    /// <summary>
    /// Gets or sets the radius.
    /// </summary>
    /// <exception cref="InvalidOperationException">Radius was less than 0.</exception>
    [JsonPropertyName("r")]
    [DataMember(Name = "r")]
    public float Radius
    {
        get => radius;
        set => radius = !float.IsNaN(value) && value >= 0 ? value : throw new InvalidOperationException("Radius should equal to or be greater than 0.");
    }

    /// <summary>
    /// Gets or sets the center point.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public PointF Center
    {
        get => center;
        set => center = value;
    }

    /// <summary>
    /// Gets the x of the center point.
    /// </summary>
    [JsonPropertyName("x")]
    [DataMember(Name = "x")]
    public float CenterX
    {
        get => Center.X;
        set => Center = new PointF(float.IsNaN(value) ? 0f : value, Center.Y);
    }

    /// <summary>
    /// Gets the x of the center point.
    /// </summary>
    [JsonPropertyName("y")]
    [DataMember(Name = "y")]
    public float CenterY
    {
        get => Center.Y;
        set => Center = new PointF(Center.X, float.IsNaN(value) ? 0f : value);
    }

    /// <summary>
    /// Gets the perimeter.
    /// </summary>
    public float Perimeter()
        => (float)(2 * Math.PI * Radius);

    /// <summary>
    /// Gets the area.
    /// </summary>
    public float Area()
        => (float)(Math.PI * Radius * Radius);

    /// <summary>
    /// Gets y by x.
    /// </summary>
    /// <param name="x">X.</param>
    /// <returns>Y.</returns>
    public (float, float) GetY(float x)
    {
        var delta = x - CenterX;
#if NETFRAMEWORK
        var r = delta == 0 ? Radius : (float)Math.Sqrt(Radius * Radius - delta * delta);
#else
        var r = delta == 0 ? Radius : MathF.Sqrt(Radius * Radius - delta * delta);
#endif
        if (float.IsNaN(r) || r > Radius) return (float.NaN, float.NaN);
        if (r == 0) return (CenterY, float.NaN);
        return (CenterY + r, CenterY - r);
    }

    /// <summary>
    /// Gets x by y.
    /// </summary>
    /// <param name="y">Y.</param>
    /// <returns>X.</returns>
    public (float, float) GetX(float y)
    {
        var delta = y - CenterY;
#if NETFRAMEWORK
        var r = delta == 0 ? Radius : (float)Math.Sqrt(Radius * Radius - delta * delta);
#else
        var r = delta == 0 ? Radius : MathF.Sqrt(Radius * Radius - delta * delta);
#endif
        if (float.IsNaN(r) || r > Radius) return (float.NaN, float.NaN);
        if (r == 0) return (CenterY, float.NaN);
        return (CenterX + r, CenterX - r);
    }

    /// <summary>
    /// Test if a point is on the line.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>true if the point is on the line; otherwise, false.</returns>
    public bool Contains(Point2D<float> point)
        => point != null && Math.Abs(Math.Pow(point.X - CenterX, 2) + Math.Pow(point.Y - CenterY, 2) - Radius * Radius) < InternalHelper.SingleAccuracy;

    /// <summary>
    /// Test if a point is on the line.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>true if the point is on the line; otherwise, false.</returns>
    public bool Contains(PointF point)
        => Math.Abs(Math.Pow(point.X - CenterX, 2) + Math.Pow(point.Y - CenterY, 2) - Radius * Radius) < InternalHelper.SingleAccuracy;

    /// <summary>
    /// Generates point collection in the specific zone and accuracy.
    /// </summary>
    /// <param name="accuracy">The step in x.</param>
    /// <returns>A point collection.</returns>
    public IEnumerable<PointF> DrawPoints(float accuracy)
        => InternalHelper.DrawPoints(this, CenterX - Radius, CenterX + Radius, accuracy);

    /// <summary>
    /// Generates point collection in the specific zone and accuracy.
    /// </summary>
    /// <param name="left">The left boundary.</param>
    /// <param name="right">The right boundary.</param>
    /// <param name="accuracy">The step in x.</param>
    /// <returns>A point collection.</returns>
    public IEnumerable<PointF> DrawPoints(float left, float right, float accuracy)
        => InternalHelper.DrawPoints(this, left, right, accuracy);

    /// <summary>
    /// Generates point collection in the specific zone and accuracy.
    /// </summary>
    /// <param name="left">The left boundary.</param>
    /// <param name="right">The right boundary.</param>
    /// <param name="accuracy">The step in x.</param>
    /// <returns>A point collection.</returns>
    IEnumerable<Point2D<float>> IPixelOutline<float>.DrawPoints(float left, float right, float accuracy)
        => InternalHelper.DrawPoints(this, left, right, accuracy).Select(ele => new Point2D<float>(ele.X, ele.Y));

    /// <summary>
    /// Returns a string that represents the line.
    /// </summary>
    /// <returns>A string that represents the line.</returns>
    public override string ToString()
    {
        try
        {
            return $"{CenterX:0.########}, {CenterY:0.########} (r {Radius:0.########})";
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

        return $"⊙ {CenterX}, {CenterY} (r {Radius})";
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>An instance copied from current one.</returns>
    public CoordinateCircle Clone()
        => new(center, float.IsNaN(Radius) ? 0d : Radius);

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
    public bool Equals(CoordinateCircleF other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return CenterX == other.CenterX && CenterY == other.CenterY && Radius == other.Radius;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(CoordinateEllipseF other)
        => other is not null && CenterX == other.CenterX && CenterY == other.CenterY && Radius == other.A && Radius == other.B;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (obj is CoordinateEllipseF ellipse) Equals(ellipse);
        return Equals(obj as CoordinateCircleF);
    }

    /// <inheritdoc />
    public override int GetHashCode()
        => Tuple.Create(CenterX, CenterY, Radius).GetHashCode();

    /// <summary>
    /// Compares two angles to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(CoordinateCircleF leftValue, CoordinateCircleF rightValue)
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
    public static bool operator !=(CoordinateCircleF leftValue, CoordinateCircleF rightValue)
    {
        if (leftValue is null && rightValue is null) return false;
        if (leftValue is null || rightValue is null) return true;
        return !leftValue.Equals(rightValue);
    }

    /// <summary>
    /// Converts to angle.
    /// </summary>
    /// <param name="value">The line.</param>
    public static explicit operator CoordinateEllipseF(CoordinateCircleF value)
        => value is null ? new() : new(value.Center, value.Radius, value.Radius);

    /// <summary>
    /// Make a circle by 3 points on periphery.
    /// </summary>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <param name="c">The third point.</param>
    /// <returns>The circle; or null, if failed.</returns>
    public static CoordinateCircleF ByPeriphery(PointF a, PointF b, PointF c)
    {
        var x1 = b.X - a.X;
        var y1 = b.Y - a.Y;
        var x2 = c.X - a.X;
        var y2 = c.Y - a.Y;
        var z1 = x1 * (a.X + b.X) + y1 * (a.Y + b.Y);
        var z2 = x2 * (a.X + c.X) + y2 * (a.Y + c.Y);
        var d = 2F * (x1 * (c.Y - b.Y) - y1 * (c.X - b.X));
        if (Math.Abs(d) < InternalHelper.SingleAccuracy) return null;
        var center = new PointF(
            (y2 * z1 - y1 * z2) / d,
            (x1 * z2 - x2 * z1) / d);
        return new CoordinateCircleF(center, Geometry.Distance(a, center));
    }

    /// <summary>
    /// Make incircle.
    /// </summary>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <param name="c">The third point.</param>
    /// <returns>The circle.</returns>
    public static CoordinateCircleF Incircle(PointF a, PointF b, PointF c)
    {
        var dx31 = c.X - a.X;
        var dy31 = c.Y - a.Y;
        var dx21 = b.X - a.X;
        var dy21 = b.Y - a.Y;
        var d31 = (float)Math.Sqrt(dx31 * dx31 + dy31 * dy31);
        var d21 = (float)Math.Sqrt(dx21 * dx21 + dy21 * dy21);
        var a1 = dx31 * d21 - dx21 * d31;
        var b1 = dy31 * d21 - dy21 * d31;
        var c1 = a1 * a.X + b1 * a.Y;
        var dx32 = c.X - b.X;
        var dy32 = c.Y - b.Y;
        var dx12 = -dx21;
        var dy12 = -dy21;
        var d32 = (float)Math.Sqrt(dx32 * dx32 + dy32 * dy32);
        var d12 = d21;
        var a2 = dx12 * d32 - dx32 * d12;
        var b2 = dy12 * d32 - dy32 * d12;
        var c2 = a2 * b.X + b2 * b.Y;
        var x = (c1 * b2 - c2 * b1) / (a1 * b2 - a2 * b1);
        var y = (c2 * a1 - c1 * a2) / (a1 * b2 - a2 * b1);
        return new CoordinateCircleF(x, y, (float)Math.Abs(dy21 * x - dx21 * y + dx21 * a.Y - dy21 * a.X) / d21);
    }
}

/// <summary>
/// Ellipse in coordinate.
/// </summary>
[DataContract]
public class CoordinateEllipseF : IPixelOutline<float>, ICoordinateTuplePoint<float>, ICloneable, IEquatable<CoordinateEllipseF>
{
    private PointF center;
    private float radiusA;
    private float radiusB;

    /// <summary>
    /// Initializes a new instance of the CoordinateEllipseF class.
    /// </summary>
    public CoordinateEllipseF()
    {
        Alpha = new Angle();
        center = new();
        radiusA = 0;
        radiusB = 0;
    }

    /// <summary>
    /// Initializes a new instance of the CoordinateEllipseF class.
    /// </summary>
    /// <param name="a">The longer radius.</param>
    /// <param name="b">The shorter radius.</param>
    public CoordinateEllipseF(float a, float b)
    {
        Alpha = new Angle();
        center = new(0, 0);
        SetRadius(a, b);
    }

    /// <summary>
    /// Initializes a new instance of the CoordinateEllipseF class.
    /// </summary>
    /// <param name="x">The x of center point.</param>
    /// <param name="y">The y of center point.</param>
    /// <param name="a">The longer radius.</param>
    /// <param name="b">The shorter radius.</param>
    public CoordinateEllipseF(float x, float y, float a, float b)
    {
        Alpha = new Angle();
        center = new(x, y);
        SetRadius(a, b);
    }

    /// <summary>
    /// Initializes a new instance of the CoordinateEllipseF class.
    /// </summary>
    /// <param name="center">The center point.</param>
    /// <param name="a">The longer radius.</param>
    /// <param name="b">The shorter radius.</param>
    public CoordinateEllipseF(PointF center, float a, float b)
    {
        Alpha = new Angle();
        this.center = center;
        SetRadius(a, b);
    }

    /// <summary>
    /// Initializes a new instance of the CoordinateEllipseF class.
    /// </summary>
    /// <param name="alpha">The angle between X-axis and the line of focuses.</param>
    /// <param name="x">The x of center point.</param>
    /// <param name="y">The y of center point.</param>
    /// <param name="a">The longer radius.</param>
    /// <param name="b">The shorter radius.</param>
    public CoordinateEllipseF(Angle alpha, float x, float y, float a, float b)
    {
        Alpha = alpha;
        center = new(x, y);
        SetRadius(a, b);
    }

    /// <summary>
    /// Initializes a new instance of the CoordinateEllipseF class.
    /// </summary>
    /// <param name="alpha">The angle between X-axis and the line of focuses.</param>
    /// <param name="center">The center point.</param>
    /// <param name="a">The longer radius.</param>
    /// <param name="b">The shorter radius.</param>
    public CoordinateEllipseF(Angle alpha, PointF center, float a, float b)
    {
        Alpha = alpha;
        this.center = center;
        SetRadius(a, b);
    }

    /// <summary>
    /// Gets or sets the angle between X-axis and the line of focuses.
    /// </summary>
    [JsonPropertyName("alpha")]
    [DataMember(Name = "alpha")]
    public Angle Alpha { get; set; }

    /// <summary>
    /// Gets or sets the longer radius.
    /// </summary>
    /// <exception cref="InvalidOperationException">The value was less than 0.</exception>
    [JsonPropertyName("a")]
    [DataMember(Name = "a")]
    public float A
    {
        get
        {
            return radiusA;
        }

        set
        {
            if (float.IsNaN(value) || value < 0) throw new InvalidOperationException("The value should equal to or be greater than 0.");
            if (value >= radiusB)
            {
                radiusA = value;
                return;
            }

            radiusA = radiusB;
            radiusB = value;
        }
    }

    /// <summary>
    /// Gets or sets the shorter radius.
    /// </summary>
    /// <exception cref="InvalidOperationException">The value was less than 0.</exception>
    [JsonPropertyName("b")]
    [DataMember(Name = "b")]
    public float B
    {
        get
        {
            return radiusB;
        }

        set
        {
            if (float.IsNaN(value) || value < 0) throw new InvalidOperationException("The value should equal to or be greater than 0.");
            if (value <= radiusA)
            {
                radiusB = value;
                return;
            }

            radiusB = radiusA;
            radiusA = value;
        }
    }

    /// <summary>
    /// Gets or sets the center point.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public PointF Center
    {
        get => center;
        set => center = value;
    }

    /// <summary>
    /// Gets the x of the center point.
    /// </summary>
    [JsonPropertyName("x")]
    [DataMember(Name = "x")]
    public float CenterX
    {
        get => Center.X;
        set => Center = new PointF(float.IsNaN(value) ? 0f : value, Center.Y);
    }

    /// <summary>
    /// Gets the x of the center point.
    /// </summary>
    [JsonPropertyName("y")]
    [DataMember(Name = "y")]
    public float CenterY
    {
        get => Center.Y;
        set => Center = new PointF(Center.X, float.IsNaN(value) ? 0f : value);
    }

    /// <summary>
    /// Gets the eccentricity.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
#if NETFRAMEWORK
    public float Eccentricity => (float)Math.Sqrt(1 - Math.Pow(B / A, 2));
#else
    public float Eccentricity => MathF.Sqrt(1 - MathF.Pow(B / A, 2));
#endif

    /// <summary>
    /// Gets the middle value h.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
#if NETFRAMEWORK
    public float H => (float)Math.Pow(A - B, 2) / (float)Math.Pow(A + B, 2);
#else
    public float H => MathF.Pow(A - B, 2) / MathF.Pow(A + B, 2);
#endif

    /// <summary>
    /// Gets the focal distance.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
#if NETFRAMEWORK
    public float C => (float)Math.Sqrt(A * A - B * B);
#else
    public float C => MathF.Sqrt(A * A - B * B);
#endif

    /// <summary>
    /// Gets the perimeter.
    /// </summary>
    public float Perimeter()
    {
        var h3 = H * 3;
#if NETFRAMEWORK
        return (float)Math.PI * (A + B) * (1 + h3 / (10 + (float)Math.Sqrt(4 - h3)));
#else
        return MathF.PI * (A + B) * (1 + h3 / (10 + MathF.Sqrt(4 - h3)));
#endif
    }

    /// <summary>
    /// Gets the area.
    /// </summary>
    public float Area()
#if NETFRAMEWORK
        => (float)Math.PI * A * B;
#else
        => MathF.PI * A * B;
#endif

    /// <summary>
    /// Gets focuses.
    /// </summary>
    /// <returns>The focuses.</returns>
    public (PointF, PointF) Focuses()
    {
        var c = C;
        if (Alpha.Degrees == 0 || Alpha.Degrees == 360)
            return (new(center.X - c, center.Y), new(center.X + c, center.Y));
        if (Alpha.Degrees == 90 || Alpha.Degrees == 270)
            return (new(center.X, center.Y - c), new(center.X, center.Y + c));
        return (
            Geometry.Rotate(new PointF(center.X - c, center.Y), new PointF(0F, 0F), Alpha),
            Geometry.Rotate(new PointF(center.X + c, center.Y), new PointF(0F, 0F), Alpha));
    }

    /// <summary>
    /// Gets y by x.
    /// </summary>
    /// <param name="x">X.</param>
    /// <returns>Y.</returns>
    public (float, float) GetY(float x)
    {
        x = Geometry.Rotate(new PointF(x, 0), center, Alpha).X;
        var width = x - CenterX;
#if NETFRAMEWORK
        var y = (float)Math.Sqrt((1 - width * width / A * A) * B * B);
#else
        var y = MathF.Sqrt((1 - width * width / A * A) * B * B);
#endif
        if (float.IsNaN(y)) return (float.NaN, float.NaN);
        if (y == 0) return (y, float.NaN);
        return (-y, y);
    }

    /// <summary>
    /// Gets x by y.
    /// </summary>
    /// <param name="y">Y.</param>
    /// <returns>X.</returns>
    public (float, float) GetX(float y)
    {
        y = Geometry.Rotate(new PointF(0, y), center, Alpha).Y;
        var height = y - CenterY;
#if NETFRAMEWORK
        var x = (float)Math.Sqrt((1 - height * height / B * B) * A * A);
#else
        var x = MathF.Sqrt((1 - height * height / B * B) * A * A);
#endif
        if (float.IsNaN(x)) return (float.NaN, float.NaN);
        if (x == 0) return (x, float.NaN);
        return (-x, x);
    }

    /// <summary>
    /// Test if a point is on the line.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>true if the point is on the line; otherwise, false.</returns>
    public bool Contains(Point2D<float> point)
        => point is not null && Contains(new PointF(point.X, point.Y));

    /// <summary>
    /// Test if a point is on the line.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>true if the point is on the line; otherwise, false.</returns>
    public bool Contains(PointF point)
    {
        if (Alpha.AbsDegrees > 0)
            point = Geometry.Rotate(point, center, Alpha);
        var width = point.X - CenterX;
        var height = point.Y - CenterY;
        return Math.Abs(width * width + height * height - 1) < InternalHelper.SingleAccuracy;
    }

    /// <summary>
    /// Generates point collection in the specific zone and accuracy.
    /// </summary>
    /// <param name="accuracy">The step in x.</param>
    /// <returns>A point collection.</returns>
    public IEnumerable<PointF> DrawPoints(float accuracy)
    {
        if (accuracy < 0) accuracy = Math.Abs(accuracy);
        if (accuracy < InternalHelper.SingleAccuracy) accuracy = InternalHelper.SingleAccuracy;
        for (var x = -A; x <= A; x += accuracy)
        {
            var width = x - CenterX;
            var y = (float)Math.Sqrt((1 - width * width / A * A) * B * B);
            if (float.IsNaN(y)) continue;
            if (y == 0) yield return Geometry.Rotate(new PointF(x, y), center, Alpha);
            yield return Geometry.Rotate(new PointF(x, -y), center, Alpha);
            yield return Geometry.Rotate(new PointF(x, y), center, Alpha);
        }
    }

    /// <summary>
    /// Generates point collection in the specific zone and accuracy.
    /// </summary>
    /// <param name="left">The left boundary.</param>
    /// <param name="right">The right boundary.</param>
    /// <param name="accuracy">The step in x.</param>
    /// <returns>A point collection.</returns>
    public IEnumerable<PointF> DrawPoints(float left, float right, float accuracy)
        => InternalHelper.DrawPoints(this, left, right, accuracy);

    /// <summary>
    /// Generates point collection in the specific zone and accuracy.
    /// </summary>
    /// <param name="left">The left boundary.</param>
    /// <param name="right">The right boundary.</param>
    /// <param name="accuracy">The step in x.</param>
    /// <returns>A point collection.</returns>
    IEnumerable<Point2D<float>> IPixelOutline<float>.DrawPoints(float left, float right, float accuracy)
        => InternalHelper.DrawPoints(this, left, right, accuracy).Select(ele => new Point2D<float>(ele.X, ele.Y));

    /// <summary>
    /// Returns a string that represents the line.
    /// </summary>
    /// <returns>A string that represents the line.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        if (CenterX == 0 || float.IsNaN(CenterX)) sb.Append("x²");
        else sb.AppendFormat("(x - {0})²", CenterX);
        sb.AppendFormat("/ {0}² + ", A);
        if (CenterY == 0 || float.IsNaN(CenterY)) sb.Append("y²");
        else sb.AppendFormat("(y - {0})²", CenterY);
        sb.AppendFormat("/ {0}² = 1", B);
        if (Alpha.AbsDegrees >= InternalHelper.SingleAccuracy) sb.AppendFormat(" (rotate {0})", Alpha);
        return sb.ToString();
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>An instance copied from current one.</returns>
    public CoordinateEllipse Clone()
        => new(Alpha, center, A, B);

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
    public bool Equals(CoordinateEllipseF other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return CenterX == other.CenterX && CenterY == other.CenterY && A == other.A && B == other.B;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(CoordinateCircleF other)
        => other is not null && CenterX == other.CenterX && CenterY == other.CenterY && A == other.Radius && B == other.Radius;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (obj is CoordinateCircleF ellipse) return Equals(ellipse);
        return Equals(obj as CoordinateEllipseF);
    }

    /// <inheritdoc />
    public override int GetHashCode()
        => Tuple.Create(CenterX, CenterY, A, B).GetHashCode();

    /// <summary>
    /// Compares two angles to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(CoordinateEllipseF leftValue, CoordinateEllipseF rightValue)
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
    public static bool operator !=(CoordinateEllipseF leftValue, CoordinateEllipseF rightValue)
    {
        if (leftValue is null && rightValue is null) return false;
        if (leftValue is null || rightValue is null) return true;
        return !leftValue.Equals(rightValue);
    }

    /// <summary>
    /// Sets A and B.
    /// </summary>
    /// <param name="a">The longer radius.</param>
    /// <param name="b">The shorter radius.</param>
    public void SetRadius(float a, float b)
    {
        a = float.IsNaN(a) ? 0 : Math.Abs(a);
        b = float.IsNaN(b) ? 0 : Math.Abs(b);
        if (b > a)
        {
            var c = a;
            a = b;
            b = c;
        }

        radiusA = a;
        radiusB = b;
    }
}

/// <summary>
/// Hyperbola in coordinate.
/// </summary>
[DataContract]
public class CoordinateHyperbolaF : IPixelOutline<float>, ICoordinateTuplePoint<float>, ICloneable, IEquatable<CoordinateHyperbolaF>
{
    private PointF center;
    private float radiusA;
    private float radiusB;

    /// <summary>
    /// Initializes a new instance of the CoordinateHyperbolaF class.
    /// </summary>
    public CoordinateHyperbolaF()
    {
        Alpha = new Angle();
        center = new(0, 0);
        radiusA = 0;
        radiusB = 0;
    }

    /// <summary>
    /// Initializes a new instance of the CoordinateHyperbolaF class.
    /// </summary>
    /// <param name="a">The longer radius.</param>
    /// <param name="b">The shorter radius.</param>
    public CoordinateHyperbolaF(float a, float b)
    {
        Alpha = new Angle();
        center = new(0, 0);
        SetRadius(a, b);
    }

    /// <summary>
    /// Initializes a new instance of the CoordinateHyperbolaF class.
    /// </summary>
    /// <param name="x">The x of center point.</param>
    /// <param name="y">The y of center point.</param>
    /// <param name="a">The longer radius.</param>
    /// <param name="b">The shorter radius.</param>
    public CoordinateHyperbolaF(float x, float y, float a, float b)
    {
        Alpha = new Angle();
        center = new(x, y);
        SetRadius(a, b);
    }

    /// <summary>
    /// Initializes a new instance of the CoordinateHyperbolaF class.
    /// </summary>
    /// <param name="center">The center point.</param>
    /// <param name="a">The longer radius.</param>
    /// <param name="b">The shorter radius.</param>
    public CoordinateHyperbolaF(PointF center, float a, float b)
    {
        Alpha = new Angle();
        this.center = center;
        SetRadius(a, b);
    }

    /// <summary>
    /// Initializes a new instance of the CoordinateHyperbolaF class.
    /// </summary>
    /// <param name="alpha">The angle between X-axis and the line of focuses.</param>
    /// <param name="x">The x of center point.</param>
    /// <param name="y">The y of center point.</param>
    /// <param name="a">The longer radius.</param>
    /// <param name="b">The shorter radius.</param>
    public CoordinateHyperbolaF(Angle alpha, float x, float y, float a, float b)
    {
        Alpha = alpha;
        center = new(x, y);
        SetRadius(a, b);
    }

    /// <summary>
    /// Initializes a new instance of the CoordinateHyperbolaF class.
    /// </summary>
    /// <param name="alpha">The angle between X-axis and the line of focuses.</param>
    /// <param name="center">The center point.</param>
    /// <param name="a">The longer radius.</param>
    /// <param name="b">The shorter radius.</param>
    public CoordinateHyperbolaF(Angle alpha, PointF center, float a, float b)
    {
        Alpha = alpha;
        this.center = center;
        SetRadius(a, b);
    }

    /// <summary>
    /// Gets or sets the angle between X-axis and the line of focuses.
    /// </summary>
    [JsonPropertyName("alpha")]
    [DataMember(Name = "alpha")]
    public Angle Alpha { get; set; }

    /// <summary>
    /// Gets or sets the longer radius.
    /// </summary>
    /// <exception cref="InvalidOperationException">The value was less than 0.</exception>
    [JsonPropertyName("a")]
    [DataMember(Name = "a")]
    public float A
    {
        get
        {
            return radiusA;
        }

        set
        {
            if (float.IsNaN(value) || value < 0) throw new InvalidOperationException("The value should equal to or be greater than 0.");
            if (value >= radiusB)
            {
                radiusA = value;
                return;
            }

            radiusA = radiusB;
            radiusB = value;
        }
    }

    /// <summary>
    /// Gets or sets the shorter radius.
    /// </summary>
    /// <exception cref="InvalidOperationException">The value was less than 0.</exception>
    [JsonPropertyName("b")]
    [DataMember(Name = "b")]
    public float B
    {
        get
        {
            return radiusB;
        }

        set
        {
            if (float.IsNaN(value) || value < 0) throw new InvalidOperationException("The value should equal to or be greater than 0.");
            if (value <= radiusA)
            {
                radiusB = value;
                return;
            }

            radiusB = radiusA;
            radiusA = value;
        }
    }

    /// <summary>
    /// Gets or sets the center point.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public PointF Center
    {
        get => center;
        set => center = value;
    }

    /// <summary>
    /// Gets the x of the center point.
    /// </summary>
    [JsonPropertyName("x")]
    [DataMember(Name = "x")]
    public float CenterX
    {
        get => Center.X;
        set => Center = new PointF(float.IsNaN(value) ? 0f : value, Center.Y);
    }

    /// <summary>
    /// Gets the x of the center point.
    /// </summary>
    [JsonPropertyName("y")]
    [DataMember(Name = "y")]
    public float CenterY
    {
        get => Center.Y;
        set => Center = new PointF(Center.X, float.IsNaN(value) ? 0f : value);
    }

    /// <summary>
    /// Gets the eccentricity.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
#if NETFRAMEWORK
    public float Eccentricity => (float)Math.Sqrt(1 + Math.Pow(B / A, 2));
#else
    public float Eccentricity => MathF.Sqrt(1 + MathF.Pow(B / A, 2));
#endif

    /// <summary>
    /// Gets the focal distance.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
#if NETFRAMEWORK
    public float C => (float)Math.Sqrt(A * A + B * B);
#else
    public float C => MathF.Sqrt(A * A + B * B);
#endif

    /// <summary>
    /// Gets focuses.
    /// </summary>
    /// <returns>The focuses.</returns>
    public (PointF, PointF) Focuses()
    {
        var c = C;
        if (Alpha.Degrees == 0 || Alpha.Degrees == 360)
            return (new(center.X - c, center.Y), new(center.X + c, center.Y));
        if (Alpha.Degrees == 90 || Alpha.Degrees == 270)
            return (new(center.X, center.Y - c), new(center.X, center.Y + c));
        return (
            Geometry.Rotate(new PointF(center.X - c, center.Y), new PointF(0F, 0F), Alpha),
            Geometry.Rotate(new PointF(center.X + c, center.Y), new PointF(0F, 0F), Alpha));
    }

    /// <summary>
    /// Gets y by x.
    /// </summary>
    /// <param name="x">X.</param>
    /// <returns>Y.</returns>
    public (float, float) GetY(float x)
    {
        x = Geometry.Rotate(new PointF(x, 0), center, Alpha).X;
        var width = x - CenterX;
#if NETFRAMEWORK
        var y = (float)Math.Sqrt((width * width / A * A - 1) * B * B);
#else
        var y = MathF.Sqrt((width * width / A * A - 1) * B * B);
#endif
        if (float.IsNaN(y)) return (float.NaN, float.NaN);
        if (y == 0) return (y, float.NaN);
        return (-y, y);
    }

    /// <summary>
    /// Gets x by y.
    /// </summary>
    /// <param name="y">Y.</param>
    /// <returns>X.</returns>
    public (float, float) GetX(float y)
    {
        y = Geometry.Rotate(new PointF(0, y), center, Alpha).Y;
        var height = y - CenterY;
#if NETFRAMEWORK
        var x = (float)Math.Sqrt((height * height / B * B + 1) * A * A);
#else
        var x = MathF.Sqrt((height * height / B * B + 1) * A * A);
#endif
        if (float.IsNaN(x)) return (float.NaN, float.NaN);
        if (x == 0) return (x, float.NaN);
        return (-x, x);
    }

    /// <summary>
    /// Test if a point is on the line.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>true if the point is on the line; otherwise, false.</returns>
    public bool Contains(PointF point)
    {
        if (Alpha.AbsDegrees > 0)
            point = Geometry.Rotate(point, center, Alpha);
        var width = point.X - CenterX;
        var height = point.Y - CenterY;
        return Math.Abs(width * width - height * height - 1) < InternalHelper.SingleAccuracy;
    }

    /// <summary>
    /// Test if a point is on the line.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>true if the point is on the line; otherwise, false.</returns>
    public bool Contains(Point2D<float> point)
        => point is not null && Contains(new PointF(point.X, point.Y));

    /// <summary>
    /// Generates point collection in the specific zone and accuracy.
    /// </summary>
    /// <param name="left">The left boundary.</param>
    /// <param name="right">The right boundary.</param>
    /// <param name="accuracy">The step in x.</param>
    /// <returns>A point collection.</returns>
    public IEnumerable<PointF> DrawPoints(float left, float right, float accuracy)
        => InternalHelper.DrawPoints(this, left, right, accuracy);

    /// <summary>
    /// Generates point collection in the specific zone and accuracy.
    /// </summary>
    /// <param name="left">The left boundary.</param>
    /// <param name="right">The right boundary.</param>
    /// <param name="accuracy">The step in x.</param>
    /// <returns>A point collection.</returns>
    IEnumerable<Point2D<float>> IPixelOutline<float>.DrawPoints(float left, float right, float accuracy)
        => InternalHelper.DrawPoints(this, left, right, accuracy).Select(ele => new Point2D<float>(ele.X, ele.Y));

    /// <summary>
    /// Returns a string that represents the line.
    /// </summary>
    /// <returns>A string that represents the line.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        if (CenterX == 0 || float.IsNaN(CenterX)) sb.Append("x²");
        else sb.AppendFormat("(x - {0})²", CenterX);
        sb.AppendFormat("/ {0}² - ", A);
        if (CenterY == 0 || float.IsNaN(CenterY)) sb.Append("y²");
        else sb.AppendFormat("(y - {0})²", CenterY);
        sb.AppendFormat("/ {0}² = 1", B);
        if (Alpha.AbsDegrees >= InternalHelper.SingleAccuracy) sb.AppendFormat(" (rotate {0})", Alpha);
        return sb.ToString();
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>An instance copied from current one.</returns>
    public CoordinateHyperbola Clone()
        => new(Alpha, center, A, B);

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
    public bool Equals(CoordinateHyperbolaF other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return CenterX == other.CenterX && CenterY == other.CenterY && A == other.A && B == other.B;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        return Equals(obj as CoordinateHyperbolaF);
    }

    /// <inheritdoc />
    public override int GetHashCode()
        => Tuple.Create(CenterX, CenterY, A, B).GetHashCode();

    /// <summary>
    /// Compares two angles to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(CoordinateHyperbolaF leftValue, CoordinateHyperbolaF rightValue)
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
    public static bool operator !=(CoordinateHyperbolaF leftValue, CoordinateHyperbolaF rightValue)
    {
        if (leftValue is null && rightValue is null) return false;
        if (leftValue is null || rightValue is null) return true;
        return !leftValue.Equals(rightValue);
    }

    /// <summary>
    /// Sets A and B.
    /// </summary>
    /// <param name="a">The longer radius.</param>
    /// <param name="b">The shorter radius.</param>
    public void SetRadius(float a, float b)
    {
        a = float.IsNaN(a) ? 0 : Math.Abs(a);
        b = float.IsNaN(b) ? 0 : Math.Abs(b);
        if (b > a)
        {
            var c = a;
            a = b;
            b = c;
        }

        radiusA = a;
        radiusB = b;
    }
}
