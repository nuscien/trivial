using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Trivial.Maths
{
    /// <summary>
    /// The line segment in coordinate.
    /// </summary>
    [DataContract]
    public class LineSegmentF : IPixelOutline<float>, ICoordinateSinglePoint<float>, ICloneable, IEquatable<LineSegmentF>
    {
        private PointF start;
        private PointF end;

        /// <summary>
        /// Initializes a new instance of the LineSegmentF class.
        /// </summary>
        public LineSegmentF()
        {
            start = new();
            end = new();
        }

        /// <summary>
        /// Initializes a new instance of the LineSegmentF class.
        /// </summary>
        /// <param name="x1">X of the start point.</param>
        /// <param name="y1">Y of the start point.</param>
        /// <param name="x2">X of the end point.</param>
        /// <param name="y2">Y of the end point.</param>
        public LineSegmentF(float x1, float y1, float x2, float y2)
        {
            start = new(x1, y1);
            end = new(x2, y2);
        }

        /// <summary>
        /// Initializes a new instance of the LineSegmentF class.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        public LineSegmentF(PointF start, PointF end)
        {
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Gets or sets the start point.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public PointF Start
        {
            get => start;
            set => start = value;
        }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public PointF End
        {
            get => end;
            set => end = value;
        }

        /// <summary>
        /// Gets the x of start point.
        /// </summary>
        [JsonPropertyName("x1")]
        [DataMember(Name = "x1")]
        public float StartX
        {
            get => Start.X;
            set => Start = new PointF(float.IsNaN(value) ? 0f : value, Start.Y);
        }

        /// <summary>
        /// Gets the x of start point.
        /// </summary>
        [JsonPropertyName("y1")]
        [DataMember(Name = "y1")]
        public float StartY
        {
            get => Start.Y;
            set => Start = new PointF(Start.X, float.IsNaN(value) ? 0f : value);
        }

        /// <summary>
        /// Gets the x of end point.
        /// </summary>
        [JsonPropertyName("x2")]
        [DataMember(Name = "x2")]
        public float EndX
        {
            get => End.X;
            set => End = new PointF(float.IsNaN(value) ? 0f : value, End.Y);
        }

        /// <summary>
        /// Gets the x of end point.
        /// </summary>
        [JsonPropertyName("y2")]
        [DataMember(Name = "y2")]
        public float EndY
        {
            get => End.Y;
            set => End = new PointF(Start.X, float.IsNaN(value) ? 0f : value);
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
#if NETCOREAPP3_1_OR_GREATER
        public float Length => MathF.Sqrt(MathF.Pow(End.X - Start.X, 2) + MathF.Pow(End.Y - Start.Y, 2));
#else
        public float Length => (float)Math.Sqrt((float)Math.Pow(End.X - Start.X, 2) + (float)Math.Pow(End.Y - Start.Y, 2));
#endif

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
        public float GetY(float x)
            => (x - StartX) / (StartX - EndX) * (StartY - EndY) + StartY;

        /// <summary>
        /// Gets x by y.
        /// </summary>
        /// <param name="y">Y.</param>
        /// <returns>X.</returns>
        public float GetX(float y)
            => (y - StartY) / (StartY - EndY) * (StartX - EndX) + StartX;

        /// <summary>
        /// Test if a point is on the line.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>true if the point is on the line; otherwise, false.</returns>
        public bool Contains(Point2D<float> point)
            => point != null && Math.Abs((point.Y - StartY) / (StartY - EndY) - (point.X - StartX) / (StartX - EndX)) < InternalHelper.SingleAccuracy;

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
        public bool Equals(LineSegmentF other)
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
            return Equals(obj as LineSegmentF);
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
        public static bool operator ==(LineSegmentF leftValue, LineSegmentF rightValue)
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
        public static bool operator !=(LineSegmentF leftValue, LineSegmentF rightValue)
        {
            if (leftValue is null && rightValue is null) return false;
            if (leftValue is null || rightValue is null) return true;
            return !leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Converts to a line.
        /// </summary>
        /// <param name="value">The line segment.</param>
        public static explicit operator StraightLineF(LineSegmentF value)
        {
            if (value is null) return null;
            var width = value.Start.X - value.End.X;
            var height = value.Start.Y - value.End.Y;
            return new(width, -height, value.Start.Y / height - value.Start.X / width);
        }
    }

    /// <summary>
    /// The straight line in coordinate.
    /// </summary>
    [DataContract]
    public class StraightLineF : IPixelOutline<float>, ICoordinateSinglePoint<float>, IEquatable<StraightLineF>
    {
        /// <summary>
        /// <para>Initializes a new instance of the StraightLineF class.</para>
        /// <para>General form: ax+by+c=0 (a≥0).</para>
        /// </summary>
        public StraightLineF()
        {
            A = 1;
            B = -1;
            C = 0;
            Slope = 1;
            Intercept = 0;
        }

        /// <summary>
        /// <para>Initializes a new instance of the StraightLineF class.</para>
        /// <para>General form: ax+by+c=0 (a≥0).</para>
        /// </summary>
        /// <param name="a">Parameter a.</param>
        /// <param name="b">Parameter b.</param>
        /// <param name="c">Parameter c.</param>
        public StraightLineF(float a, float b, float c)
        {
            A = a;
            B = b;
            C = c;
            Slope = GetSlope(a, b);
            Intercept = b == 0 || float.IsNaN(b) ? float.NaN : -c / b;
        }

        /// <summary>
        /// <para>Initializes a new instance of the StraightLineF class.</para>
        /// <para>Slope intercept form: y=kx+b.</para>
        /// </summary>
        /// <param name="k">Parameter k.</param>
        /// <param name="b">Parameter b.</param>
        public StraightLineF(float k, float b)
        {
            Slope = k;
            Intercept = b;
            B = -1;
            A = k;
            C = b;
        }

        /// <summary>
        /// <para>Initializes a new instance of the StraightLineF class.</para>
        /// </summary>
        /// <param name="line">The line segment to extend.</param>
        public StraightLineF(LineSegmentF line)
            : this(line.Start, line.End)
        {
        }

        /// <summary>
        /// <para>Initializes a new instance of the StraightLineF class.</para>
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        public StraightLineF(PointF a, PointF b)
        {
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
            Intercept = B == 0 || float.IsNaN(B) ? float.NaN : -C / B;
        }

        /// <summary>
        /// Gets or sets parameter a in general form.
        /// </summary>
        [JsonPropertyName("a")]
        [DataMember(Name = "a")]
        public float A { get; }

        /// <summary>
        /// Gets or sets parameter b in general form.
        /// </summary>
        [JsonPropertyName("b")]
        [DataMember(Name = "b")]
        public float B { get; }

        /// <summary>
        /// Gets or sets parameter c in general form.
        /// </summary>
        [JsonPropertyName("c")]
        [DataMember(Name = "c")]
        public float C { get; }

        /// <summary>
        /// Gets or sets the slope. Parameter k in slope intercept form.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public float Slope { get; }

        /// <summary>
        /// Gets or sets the intercept. Parameter b in slope intercept form.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public float Intercept { get; }

        /// <summary>
        /// Gets y by x.
        /// </summary>
        /// <param name="x">X.</param>
        /// <returns>Y.</returns>
        public float GetY(float x)
            => -(A * x + C) / B;

        /// <summary>
        /// Gets x by y.
        /// </summary>
        /// <param name="y">Y.</param>
        /// <returns>X.</returns>
        public float GetX(float y)
            => -(B * y + C) / A;

        /// <summary>
        /// Test if a point is on the line.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>true if the point is on the line; otherwise, false.</returns>
        public bool Contains(Point2D<float> point)
            => point != null && Math.Abs(A * point.X + B * point.Y + C) < InternalHelper.SingleAccuracy;

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
        public bool Equals(StraightLineF other)
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
            return Equals(obj as StraightLineF);
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
        public static bool operator ==(StraightLineF leftValue, StraightLineF rightValue)
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
        public static bool operator !=(StraightLineF leftValue, StraightLineF rightValue)
        {
            if (leftValue is null && rightValue is null) return false;
            if (leftValue is null || rightValue is null) return true;
            return !leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Converts to angle.
        /// </summary>
        /// <param name="value">The line.</param>
        public static explicit operator Angle(StraightLineF value)
            => value is null ? new Angle(0) : Geometry.Angle(value);

        private static float GetSlope(float a, float b)
        {
            if (b == 0 || float.IsNaN(b) || (b <= InternalHelper.SingleAccuracy && b >= -InternalHelper.SingleAccuracy))
            {
                if (a > InternalHelper.SingleAccuracy) return float.PositiveInfinity;
                if (a < -InternalHelper.SingleAccuracy) return float.NegativeInfinity;
                return float.NaN;
            }

            return -a / b;
        }
    }
}
