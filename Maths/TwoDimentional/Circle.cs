using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Trivial.Maths
{
    /// <summary>
    /// The relationship between 2 circles.
    /// </summary>
    public enum RelationshipBetweenCircles
    {
        /// <summary>
        /// Congruence.
        /// </summary>
        Congruence = 0,

        /// <summary>
        /// Separation.
        /// </summary>
        Separation = 1,

        /// <summary>
        /// Externally tangent.
        /// </summary>
        ExternallyTangent = 2,

        /// <summary>
        /// Intersection.
        /// </summary>
        Intersection = 3,

        /// <summary>
        /// Inscribe.
        /// </summary>
        Inscribe = 4,

        /// <summary>
        /// Inclusion
        /// </summary>
        Inclusion = 5
    }

    /// <summary>
    /// The circle in coordinate.
    /// </summary>
    [DataContract]
    public class CoordinateCircle : IPixelOutline<double>, ICoordinateTuplePoint<double>, ICloneable, IEquatable<CoordinateCircle>
    {
        private DoublePoint2D center;
        private double radius;

        /// <summary>
        /// Initializes a new instance of the CoordinateCircle class.
        /// </summary>
        public CoordinateCircle()
        {
            center = new();
            radius = 0;
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateCircle class.
        /// </summary>
        /// <param name="r">The radius.</param>
        public CoordinateCircle(double r)
        {
            center = new(0, 0);
            radius = double.IsNaN(r) ? 0 : Math.Abs(r);
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateCircle class.
        /// </summary>
        /// <param name="x">The x of center point.</param>
        /// <param name="y">The y of center point.</param>
        /// <param name="r">The radius.</param>
        public CoordinateCircle(double x, double y, double r)
        {
            center = new(x, y);
            radius = double.IsNaN(r) ? 0 : Math.Abs(r);
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateCircle class.
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="r">The radius.</param>
        public CoordinateCircle(DoublePoint2D center, double r)
        {
            this.center = center ?? new();
            radius = double.IsNaN(r) ? 0 : Math.Abs(r);
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateCircle class.
        /// </summary>
        /// <param name="circle">The circle.</param>
        public CoordinateCircle(CoordinateCircleF circle)
        {
            if (circle == null) circle = new();
            center = circle.Center;
            radius = circle.Radius;
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <exception cref="InvalidOperationException">Radius was less than 0.</exception>
        [JsonPropertyName("r")]
        [DataMember(Name = "r")]
        public double Radius
        {
            get => radius;
            set => radius = !double.IsNaN(value) && value >= 0 ? value : throw new InvalidOperationException("Radius should equal to or be greater than 0.");
        }

        /// <summary>
        /// Gets or sets the center point.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DoublePoint2D Center
        {
            get => center;
            set => center = value ?? new();
        }

        /// <summary>
        /// Gets the x of the center point.
        /// </summary>
        [JsonPropertyName("x")]
        [DataMember(Name = "x")]
        public double CenterX
        {
            get => Center.X;
            set => Center.X = value;
        }

        /// <summary>
        /// Gets the x of the center point.
        /// </summary>
        [JsonPropertyName("y")]
        [DataMember(Name = "y")]
        public double CenterY
        {
            get => Center.Y;
            set => Center.Y = value;
        }

        /// <summary>
        /// Gets the perimeter.
        /// </summary>
        public double Perimeter()
            => 2 * Math.PI * Radius;

        /// <summary>
        /// Gets the area.
        /// </summary>
        public double Area()
            => Math.PI * Radius * Radius;

        /// <summary>
        /// Gets y by x.
        /// </summary>
        /// <param name="x">X.</param>
        /// <returns>Y.</returns>
        public (double, double) GetY(double x)
        {
            var delta = x - CenterX;
            var r = delta == 0 ? Radius : Math.Sqrt(Radius * Radius - delta * delta);
            if (double.IsNaN(r) || r > Radius) return (double.NaN, double.NaN);
            if (r == 0) return (CenterY, double.NaN);
            return (CenterY + r, CenterY - r);
        }

        /// <summary>
        /// Gets x by y.
        /// </summary>
        /// <param name="y">Y.</param>
        /// <returns>X.</returns>
        public (double, double) GetX(double y)
        {
            var delta = y - CenterY;
            var r = delta == 0 ? Radius : Math.Sqrt(Radius * Radius - delta * delta);
            if (double.IsNaN(r) || r > Radius) return (double.NaN, double.NaN);
            if (r == 0) return (CenterY, double.NaN);
            return (CenterX + r, CenterX - r);
        }

        /// <summary>
        /// Test if a point is on the line.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>true if the point is on the line; otherwise, false.</returns>
        public bool Contains(Point2D<double> point)
            => point != null && Math.Abs(Math.Pow(point.X - CenterX, 2) + Math.Pow(point.Y - CenterY, 2) - Radius * Radius) < InternalHelper.DoubleAccuracy;

        /// <summary>
        /// Generates point collection in the specific zone and accuracy.
        /// </summary>
        /// <param name="accuracy">The step in x.</param>
        /// <returns>A point collection.</returns>
        public IEnumerable<DoublePoint2D> DrawPoints(double accuracy)
            => InternalHelper.DrawPoints(this, CenterX - Radius, CenterX + Radius, accuracy);

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
            => new(center, double.IsNaN(Radius) ? 0d : Radius);

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
        public bool Equals(CoordinateCircle other)
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
        public bool Equals(CoordinateEllipse other)
            => other is not null && CenterX == other.CenterX && CenterY == other.CenterY && Radius == other.A && Radius == other.B;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj is CoordinateEllipse ellipse) Equals(ellipse);
            return Equals(obj as CoordinateCircle);
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
        public static bool operator ==(CoordinateCircle leftValue, CoordinateCircle rightValue)
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
        public static bool operator !=(CoordinateCircle leftValue, CoordinateCircle rightValue)
        {
            if (leftValue is null && rightValue is null) return false;
            if (leftValue is null || rightValue is null) return true;
            return !leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Converts to angle.
        /// </summary>
        /// <param name="value">The line.</param>
        public static explicit operator CoordinateEllipse(CoordinateCircle value)
            => value is null ? new() : new(value.Center, value.Radius, value.Radius);

        /// <summary>
        /// Make a circle by 3 points on periphery.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="c">The third point.</param>
        /// <returns>The circle; or null, if failed.</returns>
        public static CoordinateCircle ByPeriphery(DoublePoint2D a, DoublePoint2D b, DoublePoint2D c)
        {
            if (a == null) a = new();
            if (b == null) b = new();
            if (c == null) c = new();
            var x1 = b.X - a.X;
            var y1 = b.Y - a.Y;
            var x2 = c.X - a.X;
            var y2 = c.Y - a.Y;
            var z1 = x1 * (a.X + b.X) + y1 * (a.Y + b.Y);
            var z2 = x2 * (a.X + c.X) + y2 * (a.Y + c.Y);
            var d = 2.0 * (x1 * (c.Y - b.Y) - y1 * (c.X - b.X));
            if (Math.Abs(d) < InternalHelper.DoubleAccuracy) return null;
            var center = new DoublePoint2D(
                (y2 * z1 - y1 * z2) / d,
                (x1 * z2 - x2 * z1) / d);
            return new CoordinateCircle(center, Geometry.Distance(a, center));
        }

        /// <summary>
        /// Make incircle.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="c">The third point.</param>
        /// <returns>The circle.</returns>
        public static CoordinateCircle Incircle(DoublePoint2D a, DoublePoint2D b, DoublePoint2D c)
        {
            var dx31 = c.X - a.X;
            var dy31 = c.Y - a.Y;
            var dx21 = b.X - a.X;
            var dy21 = b.Y - a.Y;
            var d31 = Math.Sqrt(dx31 * dx31 + dy31 * dy31);
            var d21 = Math.Sqrt(dx21 * dx21 + dy21 * dy21);
            var a1 = dx31 * d21 - dx21 * d31;
            var b1 = dy31 * d21 - dy21 * d31;
            var c1 = a1 * a.X + b1 * a.Y;
            var dx32 = c.X - b.X;
            var dy32 = c.Y - b.Y;
            var dx12 = -dx21;
            var dy12 = -dy21;
            var d32 = Math.Sqrt(dx32 * dx32 + dy32 * dy32);
            var d12 = d21;
            var a2 = dx12 * d32 - dx32 * d12;
            var b2 = dy12 * d32 - dy32 * d12;
            var c2 = a2 * b.X + b2 * b.Y;
            var x = (c1 * b2 - c2 * b1) / (a1 * b2 - a2 * b1);
            var y = (c2 * a1 - c1 * a2) / (a1 * b2 - a2 * b1);
            return new CoordinateCircle(x, y, Math.Abs(dy21 * x - dx21 * y + dx21 * a.Y - dy21 * a.X) / d21);
        }
    }

    /// <summary>
    /// Ellipse in coordinate.
    /// </summary>
    [DataContract]
    public class CoordinateEllipse : IPixelOutline<double>, ICoordinateTuplePoint<double>, ICloneable, IEquatable<CoordinateEllipse>
    {
        private DoublePoint2D center;
        private double radiusA;
        private double radiusB;

        /// <summary>
        /// Initializes a new instance of the CoordinateEllipse class.
        /// </summary>
        public CoordinateEllipse()
        {
            Alpha = new Angle();
            center = new();
            radiusA = 0;
            radiusB = 0;
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateEllipse class.
        /// </summary>
        /// <param name="a">The longer radius.</param>
        /// <param name="b">The shorter radius.</param>
        public CoordinateEllipse(double a, double b)
        {
            Alpha = new Angle();
            center = new(0, 0);
            SetRadius(a, b);
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateEllipse class.
        /// </summary>
        /// <param name="x">The x of center point.</param>
        /// <param name="y">The y of center point.</param>
        /// <param name="a">The longer radius.</param>
        /// <param name="b">The shorter radius.</param>
        public CoordinateEllipse(double x, double y, double a, double b)
        {
            Alpha = new Angle();
            center = new(x, y);
            SetRadius(a, b);
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateEllipse class.
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="a">The longer radius.</param>
        /// <param name="b">The shorter radius.</param>
        public CoordinateEllipse(DoublePoint2D center, double a, double b)
        {
            Alpha = new Angle();
            this.center = center;
            SetRadius(a, b);
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateEllipse class.
        /// </summary>
        /// <param name="alpha">The angle between X-axis and the line of focuses.</param>
        /// <param name="x">The x of center point.</param>
        /// <param name="y">The y of center point.</param>
        /// <param name="a">The longer radius.</param>
        /// <param name="b">The shorter radius.</param>
        public CoordinateEllipse(Angle alpha, double x, double y, double a, double b)
        {
            Alpha = alpha;
            center = new(x, y);
            SetRadius(a, b);
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateEllipse class.
        /// </summary>
        /// <param name="alpha">The angle between X-axis and the line of focuses.</param>
        /// <param name="center">The center point.</param>
        /// <param name="a">The longer radius.</param>
        /// <param name="b">The shorter radius.</param>
        public CoordinateEllipse(Angle alpha, DoublePoint2D center, double a, double b)
        {
            Alpha = alpha;
            this.center = center;
            SetRadius(a, b);
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateEllipse class.
        /// </summary>
        /// <param name="ellipse">The Ellipse.</param>
        public CoordinateEllipse(CoordinateEllipseF ellipse)
        {
            if (ellipse == null) ellipse = new();
            Alpha = ellipse.Alpha;
            center = ellipse.Center;
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
        public double A
        {
            get
            {
                return radiusA;
            }

            set
            {
                if (double.IsNaN(value) || value < 0) throw new InvalidOperationException("The value should equal to or be greater than 0.");
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
        public double B
        {
            get
            {
                return radiusB;
            }

            set
            {
                if (double.IsNaN(value) || value < 0) throw new InvalidOperationException("The value should equal to or be greater than 0.");
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
        public DoublePoint2D Center
        {
            get => center;
            set => center = value ?? new();
        }

        /// <summary>
        /// Gets the x of the center point.
        /// </summary>
        [JsonPropertyName("x")]
        [DataMember(Name = "x")]
        public double CenterX
        {
            get => Center.X;
            set => Center.X = value;
        }

        /// <summary>
        /// Gets the x of the center point.
        /// </summary>
        [JsonPropertyName("y")]
        [DataMember(Name = "y")]
        public double CenterY
        {
            get => Center.Y;
            set => Center.Y = value;
        }

        /// <summary>
        /// Gets the eccentricity.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public double Eccentricity => Math.Sqrt(1 - Math.Pow(B / A, 2));

        /// <summary>
        /// Gets the middle value h.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public double H => Math.Pow(A - B, 2) / Math.Pow(A + B, 2);

        /// <summary>
        /// Gets the focal distance.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public double C => Math.Sqrt(A * A - B * B);

        /// <summary>
        /// Gets the perimeter.
        /// </summary>
        public double Perimeter()
        {
            var h3 = H * 3;
            return Math.PI * (A + B) * (1 + h3 / (10 + Math.Sqrt(4 - h3)));
        }

        /// <summary>
        /// Gets the area.
        /// </summary>
        public double Area()
            => Math.PI * A * B;

        /// <summary>
        /// Gets focuses.
        /// </summary>
        /// <returns>The focuses.</returns>
        public (DoublePoint2D, DoublePoint2D) Focuses()
        {
            var c = C;
            if (Alpha.Degrees == 0 || Alpha.Degrees == 360)
                return (new(center.X - c, center.Y), new(center.X + c, center.Y));
            if (Alpha.Degrees == 90 || Alpha.Degrees == 270)
                return (new(center.X, center.Y - c), new(center.X, center.Y + c));
            return (
                Geometry.Rotate(new DoublePoint2D(center.X - c, center.Y), null, Alpha),
                Geometry.Rotate(new DoublePoint2D(center.X + c, center.Y), null, Alpha));
        }

        /// <summary>
        /// Gets y by x.
        /// </summary>
        /// <param name="x">X.</param>
        /// <returns>Y.</returns>
        public (double, double) GetY(double x)
        {
            x = Geometry.Rotate(new DoublePoint2D(x, 0), center, Alpha).X;
            var width = x - CenterX;
            var y = Math.Sqrt((1 - width * width / A * A) * B * B);
            if (double.IsNaN(y)) return (double.NaN, double.NaN);
            if (y == 0) return (y, double.NaN);
            return (-y, y);
        }

        /// <summary>
        /// Gets x by y.
        /// </summary>
        /// <param name="y">Y.</param>
        /// <returns>X.</returns>
        public (double, double) GetX(double y)
        {
            y = Geometry.Rotate(new DoublePoint2D(0, y), center, Alpha).Y;
            var height = y - CenterY;
            var x = Math.Sqrt((1 - height * height / B * B) * A * A);
            if (double.IsNaN(x)) return (double.NaN, double.NaN);
            if (x == 0) return (x, double.NaN);
            return (-x, x);
        }

        /// <summary>
        /// Test if a point is on the line.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>true if the point is on the line; otherwise, false.</returns>
        public bool Contains(Point2D<double> point)
        {
            if (point == null) return false;
            if (Alpha.AbsDegrees > 0)
                point = Geometry.Rotate(point, center, Alpha);
            var width = point.X - CenterX;
            var height = point.Y - CenterY;
            return Math.Abs(width * width + height * height - 1) < InternalHelper.DoubleAccuracy;
        }

        /// <summary>
        /// Generates point collection in the specific zone and accuracy.
        /// </summary>
        /// <param name="accuracy">The step in x.</param>
        /// <returns>A point collection.</returns>
        public IEnumerable<DoublePoint2D> DrawPoints(double accuracy)
        {
            if (accuracy < 0) accuracy = Math.Abs(accuracy);
            if (accuracy < InternalHelper.DoubleAccuracy) accuracy = InternalHelper.DoubleAccuracy;
            for (var x = -A; x <= A; x += accuracy)
            {
                var width = x - CenterX;
                var y = Math.Sqrt((1 - width * width / A * A) * B * B);
                if (double.IsNaN(y)) continue;
                if (y == 0) yield return Geometry.Rotate(new Point2D<double>(x, y), center, Alpha);
                yield return Geometry.Rotate(new Point2D<double>(x, -y), center, Alpha);
                yield return Geometry.Rotate(new Point2D<double>(x, y), center, Alpha);
            }
        }

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
            var sb = new StringBuilder();
            if (CenterX == 0 || double.IsNaN(CenterX)) sb.Append("x²");
            else sb.AppendFormat("(x - {0})²", CenterX);
            sb.AppendFormat("/ {0}² + ", A);
            if (CenterY == 0 || double.IsNaN(CenterY)) sb.Append("y²");
            else sb.AppendFormat("(y - {0})²", CenterY);
            sb.AppendFormat("/ {0}² = 1", B);
            if (Alpha.AbsDegrees >= InternalHelper.DoubleAccuracy) sb.AppendFormat(" (rotate {0})", Alpha);
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
        public bool Equals(CoordinateEllipse other)
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
        public bool Equals(CoordinateCircle other)
            => other is not null && CenterX == other.CenterX && CenterY == other.CenterY && A == other.Radius && B == other.Radius;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj is CoordinateCircle ellipse) return Equals(ellipse);
            return Equals(obj as CoordinateEllipse);
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
        public static bool operator ==(CoordinateEllipse leftValue, CoordinateEllipse rightValue)
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
        public static bool operator !=(CoordinateEllipse leftValue, CoordinateEllipse rightValue)
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
        public void SetRadius(double a, double b)
        {
            a = double.IsNaN(a) ? 0 : Math.Abs(a);
            b = double.IsNaN(b) ? 0 : Math.Abs(b);
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
    public class CoordinateHyperbola : IPixelOutline<double>, ICoordinateTuplePoint<double>, ICloneable, IEquatable<CoordinateHyperbola>
    {
        private DoublePoint2D center;
        private double radiusA;
        private double radiusB;

        /// <summary>
        /// Initializes a new instance of the CoordinateHyperbola class.
        /// </summary>
        public CoordinateHyperbola()
        {
            Alpha = new Angle();
            center = new(0, 0);
            radiusA = 0;
            radiusB = 0;
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateHyperbola class.
        /// </summary>
        /// <param name="a">The longer radius.</param>
        /// <param name="b">The shorter radius.</param>
        public CoordinateHyperbola(double a, double b)
        {
            Alpha = new Angle();
            center = new(0, 0);
            SetRadius(a, b);
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateHyperbola class.
        /// </summary>
        /// <param name="x">The x of center point.</param>
        /// <param name="y">The y of center point.</param>
        /// <param name="a">The longer radius.</param>
        /// <param name="b">The shorter radius.</param>
        public CoordinateHyperbola(double x, double y, double a, double b)
        {
            Alpha = new Angle();
            center = new(x, y);
            SetRadius(a, b);
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateHyperbola class.
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="a">The longer radius.</param>
        /// <param name="b">The shorter radius.</param>
        public CoordinateHyperbola(DoublePoint2D center, double a, double b)
        {
            Alpha = new Angle();
            this.center = center ?? new(0, 0);
            SetRadius(a, b);
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateHyperbola class.
        /// </summary>
        /// <param name="alpha">The angle between X-axis and the line of focuses.</param>
        /// <param name="x">The x of center point.</param>
        /// <param name="y">The y of center point.</param>
        /// <param name="a">The longer radius.</param>
        /// <param name="b">The shorter radius.</param>
        public CoordinateHyperbola(Angle alpha, double x, double y, double a, double b)
        {
            Alpha = alpha;
            center = new(x, y);
            SetRadius(a, b);
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateHyperbola class.
        /// </summary>
        /// <param name="alpha">The angle between X-axis and the line of focuses.</param>
        /// <param name="center">The center point.</param>
        /// <param name="a">The longer radius.</param>
        /// <param name="b">The shorter radius.</param>
        public CoordinateHyperbola(Angle alpha, DoublePoint2D center, double a, double b)
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
        public double A
        {
            get
            {
                return radiusA;
            }

            set
            {
                if (double.IsNaN(value) || value < 0) throw new InvalidOperationException("The value should equal to or be greater than 0.");
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
        public double B
        {
            get
            {
                return radiusB;
            }

            set
            {
                if (double.IsNaN(value) || value < 0) throw new InvalidOperationException("The value should equal to or be greater than 0.");
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
        public DoublePoint2D Center
        {
            get => center;
            set => center = value ?? new();
        }

        /// <summary>
        /// Gets the x of the center point.
        /// </summary>
        [JsonPropertyName("x")]
        [DataMember(Name = "x")]
        public double CenterX
        {
            get => Center.X;
            set => Center.X = value;
        }

        /// <summary>
        /// Gets the x of the center point.
        /// </summary>
        [JsonPropertyName("y")]
        [DataMember(Name = "y")]
        public double CenterY
        {
            get => Center.Y;
            set => Center.Y = value;
        }

        /// <summary>
        /// Gets the eccentricity.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public double Eccentricity => Math.Sqrt(1 + Math.Pow(B / A, 2));

        /// <summary>
        /// Gets the focal distance.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public double C => Math.Sqrt(A * A + B * B);

        /// <summary>
        /// Gets focuses.
        /// </summary>
        /// <returns>The focuses.</returns>
        public (DoublePoint2D, DoublePoint2D) Focuses()
        {
            var c = C;
            if (Alpha.Degrees == 0 || Alpha.Degrees == 360)
                return (new(center.X - c, center.Y), new(center.X + c, center.Y));
            if (Alpha.Degrees == 90 || Alpha.Degrees == 270)
                return (new(center.X, center.Y - c), new(center.X, center.Y + c));
            return (
                Geometry.Rotate(new DoublePoint2D(center.X - c, center.Y), null, Alpha),
                Geometry.Rotate(new DoublePoint2D(center.X + c, center.Y), null, Alpha));
        }

        /// <summary>
        /// Gets y by x.
        /// </summary>
        /// <param name="x">X.</param>
        /// <returns>Y.</returns>
        public (double, double) GetY(double x)
        {
            x = Geometry.Rotate(new DoublePoint2D(x, 0), center, Alpha).X;
            var width = x - CenterX;
            var y = Math.Sqrt((width * width / A * A - 1) * B * B);
            if (double.IsNaN(y)) return (double.NaN, double.NaN);
            if (y == 0) return (y, double.NaN);
            return (-y, y);
        }

        /// <summary>
        /// Gets x by y.
        /// </summary>
        /// <param name="y">Y.</param>
        /// <returns>X.</returns>
        public (double, double) GetX(double y)
        {
            y = Geometry.Rotate(new DoublePoint2D(0, y), center, Alpha).Y;
            var height = y - CenterY;
            var x = Math.Sqrt((height * height / B * B + 1) * A * A);
            if (double.IsNaN(x)) return (double.NaN, double.NaN);
            if (x == 0) return (x, double.NaN);
            return (-x, x);
        }

        /// <summary>
        /// Test if a point is on the line.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>true if the point is on the line; otherwise, false.</returns>
        public bool Contains(Point2D<double> point)
        {
            if (point == null) return false;
            if (Alpha.AbsDegrees > 0)
                point = Geometry.Rotate(point, center, Alpha);
            var width = point.X - CenterX;
            var height = point.Y - CenterY;
            return Math.Abs(width * width - height * height - 1) < InternalHelper.DoubleAccuracy;
        }

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
            var sb = new StringBuilder();
            if (CenterX == 0 || double.IsNaN(CenterX)) sb.Append("x²");
            else sb.AppendFormat("(x - {0})²", CenterX);
            sb.AppendFormat("/ {0}² - ", A);
            if (CenterY == 0 || double.IsNaN(CenterY)) sb.Append("y²");
            else sb.AppendFormat("(y - {0})²", CenterY);
            sb.AppendFormat("/ {0}² = 1", B);
            if (Alpha.AbsDegrees >= InternalHelper.DoubleAccuracy) sb.AppendFormat(" (rotate {0})", Alpha);
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
        public bool Equals(CoordinateHyperbola other)
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
            return Equals(obj as CoordinateHyperbola);
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
        public static bool operator ==(CoordinateHyperbola leftValue, CoordinateHyperbola rightValue)
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
        public static bool operator !=(CoordinateHyperbola leftValue, CoordinateHyperbola rightValue)
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
        public void SetRadius(double a, double b)
        {
            a = double.IsNaN(a) ? 0 : Math.Abs(a);
            b = double.IsNaN(b) ? 0 : Math.Abs(b);
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
}
