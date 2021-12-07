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
    /// The line segment.
    /// </summary>
    [DataContract]
    public class Circle
    {
        private DoubleTwoDimensionalPoint center;

        /// <summary>
        /// Initializes a new instance of the Circle class.
        /// </summary>
        public Circle()
        {
            center = new();
            Radius = 0;
        }

        /// <summary>
        /// Initializes a new instance of the Circle class.
        /// </summary>
        /// <param name="x">The x of center point.</param>
        /// <param name="y">The y of center point.</param>
        /// <param name="r">The radius.</param>
        public Circle(double x, double y, double r)
        {
            center = new(x, y);
            Radius = double.IsNaN(r) ? 0 : r;
        }

        /// <summary>
        /// Initializes a new instance of the Circle class.
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="r">The radius.</param>
        public Circle(DoubleTwoDimensionalPoint center, double r)
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
        /// Gets the x of start point.
        /// </summary>
        [JsonPropertyName("x")]
        [DataMember(Name = "x")]
        public double X
        {
            get => Center.X;
            set => Center.X = value;
        }

        /// <summary>
        /// Gets the x of start point.
        /// </summary>
        [JsonPropertyName("y")]
        [DataMember(Name = "y")]
        public double Y
        {
            get => Center.Y;
            set => Center.Y = value;
        }

        /// <summary>
        /// Gets the perimeter.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public double Perimeter => 2 * Math.PI * Radius;

        /// <summary>
        /// Gets the area.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public double Area => Math.PI * Radius * Radius;

        /// <summary>
        /// Returns a string that represents the line.
        /// </summary>
        /// <returns>A string that represents the line.</returns>
        public override string ToString()
        {
            try
            {
                return $"{X:0.########}, {Y:0.########} (r {Radius:0.########})";
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

            return $"{X}, {Y} (r {Radius})";
        }

        /// <summary>
        /// Make a circle by 3 points on periphery.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="c">The third point.</param>
        /// <returns>The circle; or null, if failed.</returns>
        public static Circle ByPeriphery(DoubleTwoDimensionalPoint a, DoubleTwoDimensionalPoint b, DoubleTwoDimensionalPoint c)
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
            if (Math.Abs(d) < Arithmetic.DoubleAccuracy) return null;
            var center = new DoubleTwoDimensionalPoint(
                (y2 * z1 - y1 * z2) / d,
                (x1 * z2 - x2 * z1) / d);
            return new Circle(center, Geometry.Distance(a, center));
        }

        /// <summary>
        /// Make incircle.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="c">The third point.</param>
        /// <returns>The circle.</returns>
        public static Circle Incircle(DoubleTwoDimensionalPoint a, DoubleTwoDimensionalPoint b, DoubleTwoDimensionalPoint c)
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
            return new Circle(x, y, Math.Abs(dy21 * x - dx21 * y + dx21 * a.Y - dy21 * a.X) / d21);
        }
    }
}
