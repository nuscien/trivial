using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Maths
{
    /// <summary>
    /// The utility for geometry.
    /// </summary>
    public static class Geometry
    {
        internal const double Accuracy = 1e-10;

        /// <summary>
        /// Computes euclidean metric.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns>The distance.</returns>
        public static double Distance(DoubleTwoDimensionalPoint start, DoubleTwoDimensionalPoint end)
        {
            if (start == null) start = new();
            if (end == null) end = new();
            return Math.Sqrt((start.X - end.X) * (start.X - end.X) + (start.Y - end.Y) * (start.Y - end.Y));
        }

        /// <summary>
        /// Tests if the 2 points are same.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool Equals(DoubleTwoDimensionalPoint a, DoubleTwoDimensionalPoint b)
        {
            if (a == null) a = new();
            if (b == null) b = new();
            return (Math.Abs(a.X - b.X) < Accuracy) && (Math.Abs(a.Y - b.Y) < Accuracy);
        }

        /// <summary>
        /// Computes vector cross product.
        /// </summary>
        /// <param name="a">A point.</param>
        /// <param name="b">Another point.</param>
        /// <param name="o">The vertex/origin point.</param>
        /// <returns>The vector cross product. Greater than 0 if anticlockwise; equals to 0 if collineation; less than 0 if clockwise.</returns>
        public static double CrossProduct(DoubleTwoDimensionalPoint a, DoubleTwoDimensionalPoint b, DoubleTwoDimensionalPoint o = null)
        {
            if (a == null) a = new();
            if (b == null) b = new();
            if (o == null) o = new();
            return (a.X - o.X) * (b.Y - o.Y) - (b.X - o.X) * (a.Y - o.Y);
        }

        /// <summary>
        /// Computes vector dot (scalar) product.
        /// </summary>
        /// <param name="a">A point.</param>
        /// <param name="b">Another point.</param>
        /// <param name="o">The vertex/origin point.</param>
        /// <returns>The vector dot (scalar) product. Greater than 0 if it is obtuse angle; equals to 0 if it is right angle; less than 0 if it is acute angle.</returns>
        public static double DotProduct(DoubleTwoDimensionalPoint a, DoubleTwoDimensionalPoint b, DoubleTwoDimensionalPoint o = null)
        {
            if (a == null) a = new();
            if (b == null) b = new();
            if (o == null) o = new();
            return (a.X - o.X) * (b.X - o.X) + (a.Y - o.Y) * (b.Y - o.Y);
        }

        /// <summary>
        /// Gets the point after counter clockwise rotation.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="origin">The origin point.</param>
        /// <param name="alpha">Radian to counter clockwise rotate.</param>
        /// <returns>The point after rotation.</returns>
        public static DoubleTwoDimensionalPoint Rotate(DoubleTwoDimensionalPoint point, DoubleTwoDimensionalPoint origin, double alpha)
        {
            if (point == null) point = new();
            if (origin == null) origin = new();
            DoubleTwoDimensionalPoint tp = new();
            point.X -= origin.X;
            point.Y -= origin.Y;
            tp.X = point.X * Math.Cos(alpha) - point.Y * Math.Sin(alpha) + origin.X;
            tp.Y = point.Y * Math.Cos(alpha) + point.X * Math.Sin(alpha) + origin.Y;
            return tp;
        }

        /// <summary>
        /// Computes included angle.
        /// </summary>
        /// <param name="vertex">The vertex point.</param>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns>The included angle radian of line vertex-start and line vertex-end.</returns>
        public static double Angle(DoubleTwoDimensionalPoint vertex, DoubleTwoDimensionalPoint start, DoubleTwoDimensionalPoint end)
        {
            if (vertex == null) vertex = new();
            if (start == null) start = new();
            if (end == null) end = new();
            double cosfi, fi, norm;
            var dsx = start.X - vertex.X;
            var dsy = start.Y - vertex.Y;
            var dex = end.X - vertex.X;
            var dey = end.Y - vertex.Y;
            if (Math.Abs(dex) < Accuracy && Math.Abs(dey) < Accuracy) return 0;
            cosfi = dsx * dex + dsy * dey;
            norm = (dsx * dsx + dsy * dsy) * (dex * dex + dey * dey);
            cosfi /= Math.Sqrt(norm);
            if (cosfi >= 1.0) return 0;
            if (cosfi <= -1.0) return -Math.PI;
            fi = Math.Acos(cosfi);
            if (dsx * dey - dsy * dex > 0) return fi;
            return -fi;
        }

        /// <summary>
        /// Test if a point is on a line.
        /// </summary>
        /// <param name="line">The line sgement.</param>
        /// <param name="point">The point to test.</param>
        /// <returns>true if the point is on the line; otherwise, false.</returns>
        public static bool IsOn(LineSegment line, DoubleTwoDimensionalPoint point)
        {
            if (line == null) return false;
            if (point == null) point = new();
            return CrossProduct(line.End, point, line.Start) == 0 && (point.X - line.Start.X) * (point.X - line.End.X) <= 0 && (point.Y - line.Start.Y) * (point.Y - line.End.Y) <= 0;
        }

        /// <summary>
        /// Gets the position of a point that relative to a specific line.
        /// </summary>
        /// <param name="point">The point to get the relative position.</param>
        /// <param name="line">The line to compare.</param>
        /// <returns>The relative position ratio. Less than 0 if the point is on the backward extension of the line segment; greater than 1 if forward; equals to 0 if on the start point of the line segment; equals to 1 if on the end point; between 0 and 1 if on the line segment.</returns>
        /// <exception cref="ArgumentNullException">line was null.</exception>
        public static double Relation(DoubleTwoDimensionalPoint point, LineSegment line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line), "line should not be null.");
            if (point == null) point = new();
            LineSegment l = new();
            l.Start = line.Start;
            l.End = point;
            return DotProduct(l.End, line.End, line.Start) / (Distance(line.Start, line.End) * Distance(line.Start, line.End));
        }

        /// <summary>
        /// Gets the foot point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="line">The line.</param>
        /// <returns>The foot point.</returns>
        /// <exception cref="ArgumentNullException">line was null.</exception>
        public static DoubleTwoDimensionalPoint GetFootPoint(DoubleTwoDimensionalPoint point, LineSegment line)
        {
            var r = Relation(point, line);
            return new(
                line.Start.X + r * (line.End.X - line.Start.X),
                line.Start.Y + r * (line.End.Y - line.Start.Y));
        }

        /// <summary>
        /// Computes the distance between the point and the line.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="line">The line.</param>
        /// <param name="closest">The foot point or the closest point.</param>
        /// <returns>The distance.</returns>
        /// <exception cref="ArgumentNullException">line was null.</exception>
        public static double Distance(DoubleTwoDimensionalPoint point, LineSegment line, out DoubleTwoDimensionalPoint closest)
        {
            if (point == null) point = new();
            var r = Relation(point, line);
            if (r < 0)
            {
                closest = line.Start;
                return Distance(point, line.Start);
            }
            if (r > 1)
            {
                closest = line.End;
                return Distance(point, line.End);
            }

            closest = new(
                line.Start.X + r * (line.End.X - line.Start.X),
                line.Start.Y + r * (line.End.Y - line.Start.Y));
            return Distance(point, closest);
        }

        /// <summary>
        /// Computes the distance between the point and the line.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="line">The line.</param>
        /// <returns>The distance.</returns>
        /// <exception cref="ArgumentNullException">line was null.</exception>
        public static double Distance(DoubleTwoDimensionalPoint point, LineSegment line)
            => Distance(point, line, out _);

        /// <summary>
        /// Computes the distance between the point and the line.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="line">The line.</param>
        /// <param name="extendLine">true if extend the line segment to a line; otherwise, false.</param>
        /// <returns>The distance.</returns>
        /// <exception cref="ArgumentNullException">line was null.</exception>
        public static double Distance(DoubleTwoDimensionalPoint point, LineSegment line, bool extendLine)
            => extendLine
            ? Math.Abs(CrossProduct(point, line.End, line.Start)) / Distance(line.Start, line.End)
            : Distance(point, line, out _);

        /// <summary>
        /// Computes the distance between the point and the polyline.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="polyline">The polyline.</param>
        /// <param name="closest">The foot point or the closest point.</param>
        /// <returns>The distance.</returns>
        public static double Distance(DoubleTwoDimensionalPoint point, DoubleTwoDimensionalPoint[] polyline, out DoubleTwoDimensionalPoint closest)
        {
            var cd = double.PositiveInfinity;
            double td;
            LineSegment l = new();
            DoubleTwoDimensionalPoint cq = new();
            var count = polyline.Length - 1;
            for (var i = 0; i < count; i++)
            {
                l.Start = polyline[i];
                l.End = polyline[i + 1];
                td = Distance(point, l, out var tq);
                if (td < cd)
                {
                    cd = td;
                    cq = tq;
                }
            }

            closest = cq;
            return cd;
        }

        /// <summary>
        /// Computes the distance between the point and the polyline.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="polyline">The polyline.</param>
        /// <returns>The distance.</returns>
        public static double Distance(DoubleTwoDimensionalPoint point, DoubleTwoDimensionalPoint[] polyline)
            => Distance(point, polyline, out _);

        /// <summary>
        /// Computes the distance between the point and the polyline.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="polyline">The polyline.</param>
        /// <param name="closest">The foot point or the closest point.</param>
        /// <returns>The distance.</returns>
        public static double Distance(DoubleTwoDimensionalPoint point, IList<DoubleTwoDimensionalPoint> polyline, out DoubleTwoDimensionalPoint closest)
        {
            var cd = double.PositiveInfinity;
            double td;
            LineSegment l = new();
            DoubleTwoDimensionalPoint cq = new();
            var count = polyline.Count - 1;
            for (var i = 0; i < count; i++)
            {
                l.Start = polyline[i];
                l.End = polyline[i + 1];
                td = Distance(point, l, out var tq);
                if (td < cd)
                {
                    cd = td;
                    cq = tq;
                }
            }
            closest = cq;
            return cd;
        }

        /// <summary>
        /// Computes the distance between the point and the polyline.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="polyline">The polyline.</param>
        /// <returns>The distance.</returns>
        public static double Distance(DoubleTwoDimensionalPoint point, IList<DoubleTwoDimensionalPoint> polyline)
            => Distance(point, polyline, out _);

        /// <summary>
        /// Tests if the circle is in the polygon or is intersected with the polygon.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="polygon">The polygon.</param>
        /// <returns>true if the circle is in the polygon or is intersected with the polygon; otherwise, false.</returns>
        /// <exception cref="ArgumentException">radius was invalid.</exception>
        public static bool IsInOrIntersected(DoubleTwoDimensionalPoint center, double radius, DoubleTwoDimensionalPoint[] polygon)
        {
            if (double.IsNaN(radius)) throw new ArgumentException("radius should be a valid number.", nameof(radius), new InvalidOperationException("radius is invalid."));
            var d = Distance(center, polygon, out _);
            return d < radius || Math.Abs(d - radius) < Accuracy;
        }

        /// <summary>
        /// Tests if the circle is in the polygon or is intersected with the polygon.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="polygon">The polygon.</param>
        /// <returns>true if the circle is in the polygon or is intersected with the polygon; otherwise, false.</returns>
        /// <exception cref="ArgumentException">radius was invalid.</exception>
        public static bool IsInOrIntersected(DoubleTwoDimensionalPoint center, double radius, IList<DoubleTwoDimensionalPoint> polygon)
        {
            if (double.IsNaN(radius)) throw new ArgumentException("radius should be a valid number.", nameof(radius), new InvalidOperationException("radius is invalid."));
            var d = Distance(center, polygon, out _);
            return d < radius || Math.Abs(d - radius) < Accuracy;
        }

        /// <summary>
        /// Computes sine of 2 vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The sine.</returns>
        /// <exception cref="ArgumentNullException">a or b was null.</exception>
        public static double Sin(LineSegment a, LineSegment b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a), "a should not be null");
            if (b == null) throw new ArgumentNullException(nameof(b), "b should not be null");
            return (Distance(a.End, a.Start) * Distance(b.End, b.Start)) / ((a.End.X - a.Start.X) * (b.End.X - b.Start.X) + (a.End.Y - a.Start.Y) * (b.End.Y - b.Start.Y));
        }

        /// <summary>
        /// Computes cosine of 2 vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The cosine.</returns>
        /// <exception cref="ArgumentNullException">a or b was null.</exception>
        public static double Cos(LineSegment a, LineSegment b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a), "a should not be null");
            if (b == null) throw new ArgumentNullException(nameof(b), "b should not be null");
            return ((a.End.X - a.Start.X) * (b.End.X - b.Start.X) + (a.End.Y - a.Start.Y) * (b.End.Y - b.Start.Y)) / (Distance(a.End, a.Start) * Distance(b.End, b.Start));
        }

        /// <summary>
        /// Computes included angle.
        /// </summary>
        /// <param name="a">The first line segment.</param>
        /// <param name="b">The second line segment.</param>
        /// <returns>The included angle radian of line vertex-start and line vertex-end.</returns>
        /// <exception cref="ArgumentNullException">a or b was null.</exception>
        public static double Angle(LineSegment a, LineSegment b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a), "a should not be null");
            if (b == null) throw new ArgumentNullException(nameof(b), "b should not be null");
            return Angle(new(0, 0), new(a.End.X - a.Start.X, a.End.Y - a.Start.Y), new(b.End.X - b.Start.X, b.End.Y - b.Start.Y));
        }

        /// <summary>
        /// Tests if 2 line segments are intersected.
        /// </summary>
        /// <param name="a">The first line.</param>
        /// <param name="b">The second line.</param>
        /// <returns>true if the 2 lines are intersected, including connect with vertex; otherwise, false.</returns>
        public static bool IsIntersected(LineSegment a, LineSegment b)
        {
            if (a == null || b == null) return false;
            return (Math.Max(a.Start.X, a.End.X) >= Math.Min(b.Start.X, b.End.X)) &&
                (Math.Max(b.Start.X, b.End.X) >= Math.Min(a.Start.X, a.End.X)) &&
                (Math.Max(a.Start.Y, a.End.Y) >= Math.Min(b.Start.Y, b.End.Y)) &&
                (Math.Max(b.Start.Y, b.End.Y) >= Math.Min(a.Start.Y, a.End.Y)) &&
                (CrossProduct(b.Start, a.End, a.Start) * CrossProduct(a.End, b.End, a.Start) >= 0) &&
                (CrossProduct(a.Start, b.End, b.Start) * CrossProduct(b.End, a.End, b.Start) >= 0);
        }

        /// <summary>
        /// Gets the angle of the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>The angle.</returns>
        /// <exception cref="ArgumentNullException">line was null.</exception>
        public static double Alpha(StraightLine line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line), "line should noe be null.");
            if (Math.Abs(line.A) < Accuracy)
                return 0;
            if (Math.Abs(line.B) < Accuracy)
                return Math.PI / 2;
            if (line.Slope > 0)
                return Math.Atan(line.Slope);
            else
                return Math.PI + Math.Atan(line.Slope);
        }

        /// <summary>
        /// Computes symmetry point from a line.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="line">The line.</param>
        /// <returns>The symmetry point.</returns>
        /// <exception cref="ArgumentNullException">line was null.</exception>
        public static DoubleTwoDimensionalPoint Symmetry(DoubleTwoDimensionalPoint point, StraightLine line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line), "line should noe be null.");
            if (point == null) point = new();
            return new DoubleTwoDimensionalPoint(
                ((line.B * line.B - line.A * line.A) * point.X - 2 * line.A * line.B * point.Y - 2 * line.A * line.C) / (line.A * line.A + line.B * line.B),
                ((line.A * line.A - line.B * line.B) * point.Y - 2 * line.A * line.B * point.X - 2 * line.B * line.C) / (line.A * line.A + line.B * line.B));
        }

        /// <summary>
        /// Tests the specific 2 point are in same side.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="line">The line.</param>
        /// <returns>true if they are in same side; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">line was null.</exception>
        public static bool AreSameSide(DoubleTwoDimensionalPoint a, DoubleTwoDimensionalPoint b, StraightLine line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line), "line should noe be null.");
            if (a == null) a = new();
            if (b == null) b = new();
            return (line.A * a.X + line.B * a.Y + line.C) *
            (line.A * b.X + line.B * b.Y + line.C) > 0;
        }


        /// <summary>
        /// Tests if 2 line are intersected.
        /// </summary>
        /// <param name="a">The first line.</param>
        /// <param name="b">The second line.</param>
        /// <param name="p">The point intersected.</param>
        /// <returns>true if they are intersected; otherwise, false.</returns>
        public static bool IsIntersected(StraightLine a, StraightLine b, out DoubleTwoDimensionalPoint p)
        {
            var d = a.A * b.B - b.A * a.B;
            if (Math.Abs(d) < Accuracy)
            {
                p = new DoubleTwoDimensionalPoint();
                return false;
            }

            p = new DoubleTwoDimensionalPoint(
                (b.C * a.B - a.C * b.B) / d,
                (b.A * a.C - a.A * b.C) / d);
            return true;
        }

        /// <summary>
        /// Tests if 2 line are intersected.
        /// </summary>
        /// <param name="a">The first line.</param>
        /// <param name="b">The second line.</param>
        /// <returns>true if they are intersected; otherwise, false.</returns>
        public static bool IsIntersected(StraightLine a, StraightLine b)
            => IsIntersected(a, b, out _);

        /// <summary>
        /// Tests if the polygon is simple.
        /// </summary>
        /// <param name="polygon">The polygon. Requires to input by anticlockwise.</param>
        /// <returns>true if it is a simple polygon; otherwise, false.</returns>
        /// <exception cref="ArgumentException">polygon was invalid because its points are less than 3.</exception>
        /// <exception cref="ArgumentNullException">polygon was null.</exception>
        public static bool IsSimple(DoubleTwoDimensionalPoint[] polygon)
        {
            if (polygon == null) throw new ArgumentNullException(nameof(polygon), "polygon should not be null.");
            if (polygon.Length < 3) throw new ArgumentException("polygon is invalid because the points are less than 3.");
            int i, cn;
            LineSegment l1 = new();
            LineSegment l2 = new();
            var count = polygon.Length;
            for (i = 0; i < count; i++)
            {
                l1.Start = polygon[i];
                l1.End = polygon[(i + 1) % count];
                cn = count - 3;
                while (cn != 0)
                {
                    l2.Start = polygon[(i + 2) % count];
                    l2.End = polygon[(i + 3) % count];
                    if (IsIntersected(l1, l2))
                        break;
                    cn--;
                }

                if (cn != 0) return false;
            }

            return true;
        }

        /// <summary>
        /// Tests if the polygon is simple.
        /// </summary>
        /// <param name="polygon">The polygon. Requires to input by anticlockwise.</param>
        /// <returns>true if it is a simple polygon; otherwise, false.</returns>
        public static bool IsSimple(IList<DoubleTwoDimensionalPoint> polygon)
        {
            int i, cn;
            LineSegment l1 = new();
            LineSegment l2 = new();
            var count = polygon.Count;
            for (i = 0; i < count; i++)
            {
                l1.Start = polygon[i];
                l1.End = polygon[(i + 1) % count];
                cn = count - 3;
                while (cn != 0)
                {
                    l2.Start = polygon[(i + 2) % count];
                    l2.End = polygon[(i + 3) % count];
                    if (IsIntersected(l1, l2))
                        break;
                    cn--;
                }

                if (cn != 0) return false;
            }

            return true;
        }

        /// <summary>
        /// Gets convexity for each point in the polygon.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <returns>The boolean array about if each point is convexity.</returns>
        public static IList<bool> Convexity(DoubleTwoDimensionalPoint[] polygon)
        {
            var index = 0;
            var len = polygon.Length;
            var point = polygon[0];
            var list = new List<bool>();
            for (var i = 1; i < len; i++)
            {
                list.Add(false);
                if (polygon[i].Y < point.Y || (polygon[i].Y == point.Y && polygon[i].X < point.X))
                {
                    point = polygon[i];
                    index = i;
                }
            }

            var count = len - 1;
            list[index] = true;
            while (count > 0)
            {
                if (CrossProduct(polygon[(index + 1) % len], polygon[(index + 2) % len], polygon[index]) >= 0)
                    list[(index + 1) % len] = true;
                else
                    list[(index + 1) % len] = false;
                index++;
                count--;
            }

            return list;
        }

        /// <summary>
        /// Tests if the polygon is convex.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <returns>true if it is a convex polygon.</returns>
        public static bool IsConvex(DoubleTwoDimensionalPoint[] polygon)
            => !Convexity(polygon).Any(ele => !ele);

        /// <summary>
        /// Computes the area of the polygon.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <returns>The area of the polygon. Greater than 0 if anticlockwise; less than 0 if clockwise.</returns>
        public static double Area(DoubleTwoDimensionalPoint[] polygon)
        {
            var count = polygon.Length;
            if (count < 3) return 0;
            var s = polygon[0].Y * (polygon[count - 1].X - polygon[1].X);
            for (var i = 1; i < count; i++)
            {
                s += polygon[i].Y * (polygon[i - 1].X - polygon[(i + 1) % count].X);
            }

            return s / 2;
        }

        /// <summary>
        /// Computes the area of the polygon.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <returns>The area of the polygon. Greater than 0 if anticlockwise; less than 0 if clockwise.</returns>
        public static double Area(IList<DoubleTwoDimensionalPoint> polygon)
        {
            var count = polygon.Count;
            if (count < 3) return 0;
            var s = polygon[0].Y * (polygon[count - 1].X - polygon[1].X);
            for (var i = 1; i < count; i++)
            {
                s += polygon[i].Y * (polygon[i - 1].X - polygon[(i + 1) % count].X);
            }

            return s / 2;
        }

        /// <summary>
        /// Computes the absolute area of the polygon.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <returns>The area of the polygon. Greater than 0 if anticlockwise; less than 0 if clockwise.</returns>
        public static double AbsArea(DoubleTwoDimensionalPoint[] polygon)
            => Math.Abs(Area(polygon));

        /// <summary>
        /// Computes the absolute area of the polygon.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <returns>The area of the polygon. Greater than 0 if anticlockwise; less than 0 if clockwise.</returns>
        public static double AbsArea(IList<DoubleTwoDimensionalPoint> polygon)
            => Math.Abs(Area(polygon));

        /// <summary>
        /// Tests if the points in polygon is anticlockwise.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <returns>true if it is anticlockwise; otherwise, false.</returns>
        public static bool IsAnticlockwise(DoubleTwoDimensionalPoint[] polygon)
            => Area(polygon) > 0;

        /// <summary>
        /// Tests if the points in polygon is anticlockwise.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <returns>true if it is anticlockwise; otherwise, false.</returns>
        public static bool IsAnticlockwise(IList<DoubleTwoDimensionalPoint> polygon)
            => Area(polygon) > 0;

        /// <summary>
        /// Gets the center of gravity of the polygon.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <returns>The center of gravity of the polygon.</returns>
        /// <exception cref="InvalidOperationException">Only supports simple polygon.</exception>
        public static DoubleTwoDimensionalPoint CenterOfGravity(DoubleTwoDimensionalPoint[] polygon)
        {
            if (!IsSimple(polygon)) throw new InvalidOperationException("Only supports simple polygon.", new NotImplementedException("Only supports simple polygon."));
            double xtr, ytr, wtr, xtl, ytl, wtl;
            DoubleTwoDimensionalPoint p1;
            DoubleTwoDimensionalPoint p2;
            xtr = ytr = wtr = 0.0;
            xtl = ytl = wtl = 0.0;
            var count = polygon.Length;
            for (int i = 0; i < count; i++)
            {
                p1 = polygon[i];
                p2 = polygon[(i + 1) % count];
                AddRegion(p1.X, p1.Y, p2.X, p2.Y, ref xtr, ref ytr, ref wtr, ref xtl, ref ytl, ref wtl); //全局变量变化处 
            }

            return new DoubleTwoDimensionalPoint(
                (wtr * xtr + wtl * xtl) / (wtr + wtl),
                (wtr * ytr + wtl * ytl) / (wtr + wtl));
        }

        /// <summary>
        /// Gets the center of gravity of the polygon.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <returns>The center of gravity of the polygon.</returns>
        /// <exception cref="InvalidOperationException">Only supports simple polygon.</exception>
        public static DoubleTwoDimensionalPoint CenterOfGravity(IList<DoubleTwoDimensionalPoint> polygon)
        {
            if (!IsSimple(polygon)) throw new InvalidOperationException("Only supports simple polygon.", new NotImplementedException("Only supports simple polygon."));
            double xtr, ytr, wtr, xtl, ytl, wtl;
            DoubleTwoDimensionalPoint p1;
            DoubleTwoDimensionalPoint p2;
            xtr = ytr = wtr = 0.0;
            xtl = ytl = wtl = 0.0;
            var count = polygon.Count;
            for (int i = 0; i < count; i++)
            {
                p1 = polygon[i];
                p2 = polygon[(i + 1) % count];
                AddRegion(p1.X, p1.Y, p2.X, p2.Y, ref xtr, ref ytr, ref wtr, ref xtl, ref ytl, ref wtl); //全局变量变化处 
            }

            return new DoubleTwoDimensionalPoint(
                (wtr * xtr + wtl * xtl) / (wtr + wtl),
                (wtr * ytr + wtl * ytl) / (wtr + wtl));
        }

        private static void AddPosPart(double x, double y, double w, ref double xtr, ref double ytr, ref double wtr)
        {
            if (Math.Abs(wtr + w) < Accuracy) return;
            xtr = (wtr * xtr + w * x) / (wtr + w);
            ytr = (wtr * ytr + w * y) / (wtr + w);
            wtr = w + wtr;
        }

        private static void AddNegPart(double x, double y, double w, ref double xtl, ref double ytl, ref double wtl)
        {
            if (Math.Abs(wtl + w) < Accuracy) return;
            xtl = (wtl * xtl + w * x) / (wtl + w);
            ytl = (wtl * ytl + w * y) / (wtl + w);
            wtl = w + wtl;
        }

        private static void AddRegion(double x1, double y1, double x2, double y2, ref double xtr, ref double ytr, ref double wtr, ref double xtl, ref double ytl, ref double wtl)
        {
            if (Math.Abs(x1 - x2) < Accuracy) return;
            if (x2 > x1)
            {
                AddPosPart((x2 + x1) / 2, y1 / 2, (x2 - x1) * y1, ref xtr, ref ytr, ref wtr);
                AddPosPart((x1 + x2 + x2) / 3, (y1 + y1 + y2) / 3, (x2 - x1) * (y2 - y1) / 2, ref xtr, ref ytr, ref wtr);
            }
            else
            {
                AddNegPart((x2 + x1) / 2, y1 / 2, (x2 - x1) * y1, ref xtl, ref ytl, ref wtl);
                AddNegPart((x1 + x2 + x2) / 3, (y1 + y1 + y2) / 3, (x2 - x1) * (y2 - y1) / 2, ref xtl, ref ytl, ref wtl);
            }
        }

        /// <summary>
        /// Gets the points of contact from the specific point to the polygon.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="polygon">The polygon.</param>
        /// <param name="right">The right point of contact.</param>
        /// <param name="left">The left point of contact.</param>
        /// <exception cref="ArgumentException">polygon was invalid because its points are less than 3.</exception>
        /// <exception cref="ArgumentNullException">polygon was null.</exception>
        /// <exception cref="InvalidOperationException">polygon was not simple.</exception>
        public static void PointTangentPoly(DoubleTwoDimensionalPoint point, DoubleTwoDimensionalPoint[] polygon, out DoubleTwoDimensionalPoint right, out DoubleTwoDimensionalPoint left)
        {
            if (!IsSimple(polygon)) throw new InvalidOperationException("Only supports simple polygon.", new NotImplementedException("Only supports simple polygon."));
            if (point == null) point = new();
            bool isLeft, isRight;
            var count = polygon.Length;
            LineSegment leftSegment = new();
            LineSegment rightSegment = new();
            right = polygon[0];
            left = polygon[0];
            for (int i = 1; i < count; i++)
            {
                leftSegment.Start = polygon[(i + count - 1) % count];
                leftSegment.End = polygon[i];
                rightSegment.Start = polygon[i];
                rightSegment.End = polygon[(i + 1) % count];
                isLeft = CrossProduct(leftSegment.End, point, leftSegment.Start) >= 0;
                isRight = CrossProduct(rightSegment.End, point, rightSegment.Start) >= 0;
                if (!isLeft && isRight)
                {
                    if (CrossProduct(polygon[i], right, point) > 0) right = polygon[i];
                }

                if (isLeft && !isRight)
                {
                    if (CrossProduct(left, polygon[i], point) > 0) left = polygon[i];
                }
            }

            return;
        }

        /// <summary>
        /// Gets the points of contact from the specific point to the polygon.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="polygon">The polygon.</param>
        /// <param name="right">The right point of contact.</param>
        /// <param name="left">The left point of contact.</param>
        /// <exception cref="ArgumentException">polygon was invalid because its points are less than 3.</exception>
        /// <exception cref="ArgumentNullException">polygon was null.</exception>
        /// <exception cref="InvalidOperationException">polygon was not simple.</exception>
        public static void PointTangentPoly(DoubleTwoDimensionalPoint point, IList<DoubleTwoDimensionalPoint> polygon, out DoubleTwoDimensionalPoint right, out DoubleTwoDimensionalPoint left)
        {
            if (!IsSimple(polygon)) throw new InvalidOperationException("Only supports simple polygon.", new NotImplementedException("Only supports simple polygon."));
            if (point == null) point = new();
            bool isLeft, isRight;
            var count = polygon.Count;
            LineSegment leftSegment = new();
            LineSegment rightSegment = new();
            right = polygon[0];
            left = polygon[0];
            for (int i = 1; i < count; i++)
            {
                leftSegment.Start = polygon[(i + count - 1) % count];
                leftSegment.End = polygon[i];
                rightSegment.Start = polygon[i];
                rightSegment.End = polygon[(i + 1) % count];
                isLeft = CrossProduct(leftSegment.End, point, leftSegment.Start) >= 0;
                isRight = CrossProduct(rightSegment.End, point, rightSegment.Start) >= 0;
                if (!isLeft && isRight)
                {
                    if (CrossProduct(polygon[i], right, point) > 0) right = polygon[i];
                }

                if (isLeft && !isRight)
                {
                    if (CrossProduct(left, polygon[i], point) > 0) left = polygon[i];
                }
            }

            return;
        }

        /// <summary>
        /// Tests if the polygon has a core.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <returns>A point of core; or null, if non-exists.</returns>
        /// <exception cref="ArgumentException">polygon was invalid because its points are less than 3.</exception>
        /// <exception cref="ArgumentNullException">polygon was null.</exception>
        /// <exception cref="InvalidOperationException">polygon was not simple.</exception>
        public static DoubleTwoDimensionalPoint GetCore(DoubleTwoDimensionalPoint[] polygon)
        {
            if (!IsSimple(polygon)) throw new InvalidOperationException("Only supports simple polygon.", new NotImplementedException("Only supports simple polygon."));
            int i, j, k;
            LineSegment l = new();
            var lineset = new List<StraightLine>();
            var count = polygon.Length;
            DoubleTwoDimensionalPoint p = null;
            for (i = 0; i < count; i++)
            {
                lineset.Add(new StraightLine(polygon[i], polygon[(i + 1) % count]));
            }

            for (i = 0; i < count; i++)
            {
                for (j = 0; j < count; j++)
                {
                    if (i == j) continue;
                    if (IsIntersected(lineset[i], lineset[j], out p))
                    {
                        for (k = 0; k < count; k++)
                        {
                            l.Start = polygon[k];
                            l.End = polygon[(k + 1) % count];
                            if (CrossProduct(p, l.End, l.Start) > 0) break;
                        }

                        if (k == count) break;
                    }
                }

                if (j < count) break;
            }

            if (i >= count) return null;
            return p;
        }

        /// <summary>
        /// Tests if the polygon has a core.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <returns>A point of core; or null, if non-exists.</returns>
        /// <exception cref="ArgumentException">polygon was invalid because its points are less than 3.</exception>
        /// <exception cref="ArgumentNullException">polygon was null.</exception>
        /// <exception cref="InvalidOperationException">polygon was not simple.</exception>
        public static DoubleTwoDimensionalPoint GetCore(IList<DoubleTwoDimensionalPoint> polygon)
        {
            if (!IsSimple(polygon)) throw new InvalidOperationException("Only supports simple polygon.", new NotImplementedException("Only supports simple polygon."));
            int i, j, k;
            LineSegment l = new();
            var lineset = new List<StraightLine>();
            var count = polygon.Count;
            DoubleTwoDimensionalPoint p = null;
            for (i = 0; i < count; i++)
            {
                lineset.Add(new StraightLine(polygon[i], polygon[(i + 1) % count]));
            }

            for (i = 0; i < count; i++)
            {
                for (j = 0; j < count; j++)
                {
                    if (i == j) continue;
                    if (IsIntersected(lineset[i], lineset[j], out p))
                    {
                        for (k = 0; k < count; k++)
                        {
                            l.Start = polygon[k];
                            l.End = polygon[(k + 1) % count];
                            if (CrossProduct(p, l.End, l.Start) > 0) break;
                        }

                        if (k == count) break;
                    }
                }

                if (j < count) break;
            }

            if (i >= count) return null;
            return p;
        }

        /// <summary>
        /// Tests if the polygon has a core.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <returns>true if has core; otherwise, false.</returns>
        public static bool HasCore(DoubleTwoDimensionalPoint[] polygon)
            => GetCore(polygon) != null;

        /// <summary>
        /// Tests if the polygon has a core.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <returns>true if has core; otherwise, false.</returns>
        public static bool HasCore(IList<DoubleTwoDimensionalPoint> polygon)
            => GetCore(polygon) != null;

        /// <summary>
        /// Tests if the point is in the circle.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <param name="center">The point of center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <returns>true if the point is in the circle; otherwise, false.</returns>
        /// <exception cref="ArgumentException">radius was invalid.</exception>
        public static bool IsIn(DoubleTwoDimensionalPoint point, DoubleTwoDimensionalPoint center, double radius)
        {
            if (point == null) point = new();
            if (center == null) center = new();
            if (double.IsNaN(radius)) throw new ArgumentException("radius should be a valid number.", nameof(radius), new InvalidOperationException("radius is invalid."));
            var d = (point.X - center.X) * (point.X - center.X) + (point.Y - center.Y) * (point.Y - center.Y);
            var r = radius * radius;
            return d < r || Math.Abs(d - r) < Accuracy;
        }

        /// <summary>
        /// Make a circle by 3 points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="c">The third point.</param>
        /// <param name="center">The point of center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <returns>true if the make circle succeeded; otherwise, false.</returns>
        public static bool MakeCircle(DoubleTwoDimensionalPoint a, DoubleTwoDimensionalPoint b, DoubleTwoDimensionalPoint c, out DoubleTwoDimensionalPoint center, out double radius)
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
            if (Math.Abs(d) < Accuracy)
            {
                center = null;
                radius = double.PositiveInfinity;
                return false;
            }

            center = new DoubleTwoDimensionalPoint(
                (y2 * z1 - y1 * z2) / d,
                (x1 * z2 - x2 * z1) / d);
            radius = Distance(a, center);
            return true;
        }

        /// <summary>
        /// Gets the rest point of the rectangle. The rectange is made by given 3 points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="c">The third point.</param>
        /// <returns>The forth point; or null if failed.</returns>
        public static DoubleTwoDimensionalPoint GetRestPoint(DoubleTwoDimensionalPoint a, DoubleTwoDimensionalPoint b, DoubleTwoDimensionalPoint c)
        {
            if (a == null) a = new();
            if (b == null) b = new();
            if (c == null) c = new();
            if (Math.Abs(DotProduct(a, b, c)) < Accuracy)
                return new DoubleTwoDimensionalPoint(a.X + b.X - c.X, a.Y + b.Y - c.Y);
            if (Math.Abs(DotProduct(a, c, b)) < Accuracy)
                return new DoubleTwoDimensionalPoint(a.X + c.X - b.X, a.Y + c.Y - b.X);
            if (Math.Abs(DotProduct(c, b, a)) < Accuracy)
                return new DoubleTwoDimensionalPoint(c.X + b.X - a.X, c.Y + b.Y - a.Y);
            return null;
        }

        /// <summary>
        /// Gets the relationship between 2 circles.
        /// </summary>
        /// <param name="center1">The center of circle 1.</param>
        /// <param name="radius1">The radius of circle 1.</param>
        /// <param name="center2">The center of circle 2.</param>
        /// <param name="radius2">The radius of circle 2.</param>
        /// <returns>The relationship between 2 circles.</returns>
        /// <exception cref="ArgumentException">radius1 or radius2 was invalid.</exception>
        public static RelationshipBetweenCircles CircleRelation(DoubleTwoDimensionalPoint center1, double radius1, DoubleTwoDimensionalPoint center2, double radius2)
        {
            if (center1 == null) center1 = new();
            if (center2 == null) center2 = new();
            if (double.IsNaN(radius1)) throw new ArgumentException("radius1 should be a valid number.", nameof(radius1), new InvalidOperationException("radius1 is invalid."));
            if (double.IsNaN(radius2)) throw new ArgumentException("radius2 should be a valid number.", nameof(radius2), new InvalidOperationException("radius2 is invalid."));
            var d = Math.Sqrt((center1.X - center2.X) * (center1.X - center2.X) + (center1.Y - center2.Y) * (center1.Y - center2.Y));
            if (d < Accuracy && Math.Abs(radius1 - radius2) < Accuracy)
                return RelationshipBetweenCircles.Congruence;
            if (Math.Abs(d - radius1 - radius2) < Accuracy)
                return RelationshipBetweenCircles.ExternallyTangent;
            if (Math.Abs(d - Math.Abs(radius1 - radius2)) < Accuracy)
                return RelationshipBetweenCircles.Inscribe;
            if (d > radius1 + radius2)
                return RelationshipBetweenCircles.Separation;
            if (d < Math.Abs(radius1 - radius2))
                return RelationshipBetweenCircles.Inclusion;
            if (Math.Abs(radius1 - radius2) < d && d < radius1 + radius2)
                return RelationshipBetweenCircles.Intersection;
            return (RelationshipBetweenCircles)7; // Error!
        }
    }
}
