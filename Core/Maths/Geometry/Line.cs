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
    /// The line segment.
    /// </summary>
    [DataContract]
    public class LineSegment
    {
        private DoubleTwoDimensionalPoint start;
        private DoubleTwoDimensionalPoint end;

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
        /// <param name="start">The start point.</param>
        /// <param name="end">Then end point.</param>
        public LineSegment(DoubleTwoDimensionalPoint start, DoubleTwoDimensionalPoint end)
        {
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Gets or sets the start point.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DoubleTwoDimensionalPoint Start
        {
            get => start;
            set => start = value ?? new();
        }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DoubleTwoDimensionalPoint End
        {
            get => end;
            set => end = value ?? new();
        }

        /// <summary>
        /// Gets the x of start point.
        /// </summary>
        [JsonPropertyName("x1")]
        [DataMember(Name = "x1")]
        public double StartX => Start.X;

        /// <summary>
        /// Gets the x of start point.
        /// </summary>
        [JsonPropertyName("y1")]
        [DataMember(Name = "y1")]
        public double StartY => Start.Y;

        /// <summary>
        /// Gets the x of start point.
        /// </summary>
        [JsonPropertyName("x2")]
        [DataMember(Name = "x2")]
        public double EndX => End.X;

        /// <summary>
        /// Gets the x of start point.
        /// </summary>
        [JsonPropertyName("y2")]
        [DataMember(Name = "y2")]
        public double EndY => End.Y;

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
    }

    /// <summary>
    /// The straight line.
    /// </summary>
    [DataContract]
    public class StraightLine
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
        public StraightLine(DoubleTwoDimensionalPoint a, DoubleTwoDimensionalPoint b)
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

        private static double GetSlope(double a, double b)
        {
            if (b == 0 || double.IsNaN(b) || (b <= Geometry.Accuracy && b >= -Geometry.Accuracy))
            {
                if (a > Geometry.Accuracy) return double.PositiveInfinity;
                if (a < -Geometry.Accuracy) return double.NegativeInfinity;
                return double.NaN;
            }

            return -a / b;
        }
    }

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
}
