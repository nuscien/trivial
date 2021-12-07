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
    public static partial class Geometry
    {
        /// <summary>
        /// Computes the distance between a point and a plane.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="a">The a parameter of plane.</param>
        /// <param name="b">The b parameter of plane.</param>
        /// <param name="c">The c parameter of plane.</param>
        /// <param name="d">The d parameter of plane.</param>
        /// <returns>The distance.</returns>
        public static double Distance(DoubleThreeDimensionalPoint point, double a, double b, double c, double d)
        {
            return Math.Abs(a * point.X + b * point.Y + c * point.Z + d) / Math.Sqrt(a * a + b * b + c * c);
        }

        /// <summary>
        /// Computes superficial area of a sphere.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <returns>The area.</returns>
        public static double SphereArea(double radius)
            => Math.PI * radius * radius * 4;

        /// <summary>
        /// Computes volume of a sphere.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <returns>The volume.</returns>
        public static double SphereVolume(double radius)
            => Math.PI * Math.Pow(radius, 3) * 4 / 3;
    }
}
