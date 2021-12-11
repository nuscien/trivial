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
    public class CoordinateCircle
    {
        private DoubleTwoDimensionalPoint center;

        /// <summary>
        /// Initializes a new instance of the CoordinateCircle class.
        /// </summary>
        public CoordinateCircle()
        {
            center = new();
            Radius = 0;
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateCircle class.
        /// </summary>
        /// <param name="r">The radius.</param>
        public CoordinateCircle(double r)
        {
            center = new(0, 0);
            Radius = double.IsNaN(r) ? 0 : r;
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
            Radius = double.IsNaN(r) ? 0 : r;
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateCircle class.
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="r">The radius.</param>
        public CoordinateCircle(DoubleTwoDimensionalPoint center, double r)
        {
            this.center = center ?? new();
            Radius = double.IsNaN(r) ? 0 : r;
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        [JsonPropertyName("r")]
        [DataMember(Name = "r")]
        public double Radius { get; set; }

        /// <summary>
        /// Gets or sets the center point.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DoubleTwoDimensionalPoint Center
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
        public bool Contains(DoubleTwoDimensionalPoint point)
            => point != null && Math.Abs(Math.Pow(point.X - CenterX, 2) + Math.Pow(point.Y - CenterY, 2) - Radius * Radius) < InternalHelper.DoubleAccuracy;

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

            return $"{CenterX}, {CenterY} (r {Radius})";
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
        public static CoordinateCircle ByPeriphery(DoubleTwoDimensionalPoint a, DoubleTwoDimensionalPoint b, DoubleTwoDimensionalPoint c)
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
            var center = new DoubleTwoDimensionalPoint(
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
        public static CoordinateCircle Incircle(DoubleTwoDimensionalPoint a, DoubleTwoDimensionalPoint b, DoubleTwoDimensionalPoint c)
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
    public class CoordinateEllipse
    {
        private DoubleTwoDimensionalPoint center;

        /// <summary>
        /// Initializes a new instance of the CoordinateEllipse class.
        /// </summary>
        public CoordinateEllipse()
        {
            Alpha = new Angle();
            center = new();
            A = 0;
            B = 0;
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
            A = double.IsNaN(a) ? 0 : a;
            B = double.IsNaN(b) ? 0 : b;
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
            A = double.IsNaN(a) ? 0 : a;
            B = double.IsNaN(b) ? 0 : b;
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateEllipse class.
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="a">The longer radius.</param>
        /// <param name="b">The shorter radius.</param>
        public CoordinateEllipse(DoubleTwoDimensionalPoint center, double a, double b)
        {
            Alpha = new Angle();
            this.center = center;
            A = double.IsNaN(a) ? 0 : a;
            B = double.IsNaN(b) ? 0 : b;
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
            A = double.IsNaN(a) ? 0 : a;
            B = double.IsNaN(b) ? 0 : b;
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateEllipse class.
        /// </summary>
        /// <param name="alpha">The angle between X-axis and the line of focuses.</param>
        /// <param name="center">The center point.</param>
        /// <param name="a">The longer radius.</param>
        /// <param name="b">The shorter radius.</param>
        public CoordinateEllipse(Angle alpha, DoubleTwoDimensionalPoint center, double a, double b)
        {
            Alpha = alpha;
            this.center = center;
            A = double.IsNaN(a) ? 0 : a;
            B = double.IsNaN(b) ? 0 : b;
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
        [JsonPropertyName("a")]
        [DataMember(Name = "a")]
        public double A { get; set; }

        /// <summary>
        /// Gets or sets the longer radius.
        /// </summary>
        [JsonPropertyName("b")]
        [DataMember(Name = "b")]
        public double B { get; set; }

        /// <summary>
        /// Gets or sets the center point.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DoubleTwoDimensionalPoint Center
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
        public (DoubleTwoDimensionalPoint, DoubleTwoDimensionalPoint) Focuses()
        {
            var c = C;
            if (Alpha.Degrees == 0 || Alpha.Degrees == 360)
                return (new(center.X - c, center.Y), new(center.X + c, center.Y));
            if (Alpha.Degrees == 90 || Alpha.Degrees == 270)
                return (new(center.X, center.Y - c), new(center.X, center.Y + c));
            return (
                Geometry.Rotate(new DoubleTwoDimensionalPoint(center.X - c, center.Y), null, Alpha),
                Geometry.Rotate(new DoubleTwoDimensionalPoint(center.X + c, center.Y), null, Alpha));
        }

        /// <summary>
        /// Returns a string that represents the line.
        /// </summary>
        /// <returns>A string that represents the line.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (CenterX == 0 || double.IsNaN(CenterX)) sb.Append("x²");
            else sb.AppendFormat("(x - {0})²", CenterX);
            sb.Append(" + ");
            if (CenterY == 0 || double.IsNaN(CenterY)) sb.Append("y²");
            else sb.AppendFormat("(y - {0})²", CenterY);
            sb.Append(" = 1");
            if (Alpha.AbsDegrees >= InternalHelper.DoubleAccuracy) sb.AppendFormat(" (rotate {0})", Alpha);
            return sb.ToString();
        }
    }

    /// <summary>
    /// Hyperbola in coordinate.
    /// </summary>
    [DataContract]
    public class CoordinateHyperbola
    {
        private DoubleTwoDimensionalPoint center;

        /// <summary>
        /// Initializes a new instance of the CoordinateHyperbola class.
        /// </summary>
        public CoordinateHyperbola()
        {
            Alpha = new Angle();
            center = new(0, 0);
            A = 0;
            B = 0;
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
            A = double.IsNaN(a) ? 0 : a;
            B = double.IsNaN(b) ? 0 : b;
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
            A = double.IsNaN(a) ? 0 : a;
            B = double.IsNaN(b) ? 0 : b;
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateHyperbola class.
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="a">The longer radius.</param>
        /// <param name="b">The shorter radius.</param>
        public CoordinateHyperbola(DoubleTwoDimensionalPoint center, double a, double b)
        {
            Alpha = new Angle();
            this.center = center ?? new(0, 0);
            A = double.IsNaN(a) ? 0 : a;
            B = double.IsNaN(b) ? 0 : b;
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
            A = double.IsNaN(a) ? 0 : a;
            B = double.IsNaN(b) ? 0 : b;
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateHyperbola class.
        /// </summary>
        /// <param name="alpha">The angle between X-axis and the line of focuses.</param>
        /// <param name="center">The center point.</param>
        /// <param name="a">The longer radius.</param>
        /// <param name="b">The shorter radius.</param>
        public CoordinateHyperbola(Angle alpha, DoubleTwoDimensionalPoint center, double a, double b)
        {
            Alpha = alpha;
            this.center = center;
            A = double.IsNaN(a) ? 0 : a;
            B = double.IsNaN(b) ? 0 : b;
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
        [JsonPropertyName("a")]
        [DataMember(Name = "a")]
        public double A { get; set; }

        /// <summary>
        /// Gets or sets the longer radius.
        /// </summary>
        [JsonPropertyName("b")]
        [DataMember(Name = "b")]
        public double B { get; set; }

        /// <summary>
        /// Gets or sets the center point.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DoubleTwoDimensionalPoint Center
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
        public (DoubleTwoDimensionalPoint, DoubleTwoDimensionalPoint) Focuses()
        {
            var c = C;
            if (Alpha.Degrees == 0 || Alpha.Degrees == 360)
                return (new(center.X - c, center.Y), new(center.X + c, center.Y));
            if (Alpha.Degrees == 90 || Alpha.Degrees == 270)
                return (new(center.X, center.Y - c), new(center.X, center.Y + c));
            return (
                Geometry.Rotate(new DoubleTwoDimensionalPoint(center.X - c, center.Y), null, Alpha),
                Geometry.Rotate(new DoubleTwoDimensionalPoint(center.X + c, center.Y), null, Alpha));
        }

        /// <summary>
        /// Returns a string that represents the line.
        /// </summary>
        /// <returns>A string that represents the line.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (CenterX == 0 || double.IsNaN(CenterX)) sb.Append("x²");
            else sb.AppendFormat("(x - {0})²", CenterX);
            sb.Append(" - ");
            if (CenterY == 0 || double.IsNaN(CenterY)) sb.Append("y²");
            else sb.AppendFormat("(y - {0})²", CenterY);
            sb.Append(" = 1");
            if (Alpha.AbsDegrees >= InternalHelper.DoubleAccuracy) sb.AppendFormat(" (rotate {0})", Alpha);
            return sb.ToString();
        }
    }
}
