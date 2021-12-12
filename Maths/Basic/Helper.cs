using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Maths
{
    internal static class InternalHelper
    {
        internal const double DoubleAccuracy = 1e-10;

        /// <summary>
        /// Generates point collection in the specific zone and accuracy.
        /// </summary>
        /// <param name="graph">The instance.</param>
        /// <param name="left">The left boundary.</param>
        /// <param name="right">The right boundary.</param>
        /// <param name="accuracy">The step in x.</param>
        /// <returns>A point collection.</returns>
        public static IEnumerable<DoubleTwoDimensionalPoint> DrawPoints(ICoordinateSinglePoint<double> graph, double left, double right, double accuracy)
        {
            if (left > right)
            {
                var temp = left;
                left = right;
                right = temp;
            }

            if (accuracy < 0) accuracy = Math.Abs(accuracy);
            if (accuracy < DoubleAccuracy) accuracy = DoubleAccuracy;
            var x2 = left;
            for (var x = left; x < right; x += accuracy)
            {
                x2 = x;
                var y = graph.GetY(x);
                if (!double.IsNaN(y)) yield return new(x, y);
            }

            if (x2 >= right) yield break;
            var y2 = graph.GetY(right);
            if (!double.IsNaN(y2)) yield return new(right, y2);
        }

        /// <summary>
        /// Generates point collection in the specific zone and accuracy.
        /// </summary>
        /// <param name="graph">The instance.</param>
        /// <param name="left">The left boundary.</param>
        /// <param name="right">The right boundary.</param>
        /// <param name="accuracy">The step in x.</param>
        /// <returns>A point collection.</returns>
        public static IEnumerable<DoubleTwoDimensionalPoint> DrawPoints(ICoordinateTuplePoint<double> graph, double left, double right, double accuracy)
        {
            if (left > right)
            {
                var temp = left;
                left = right;
                right = temp;
            }

            if (accuracy < 0) accuracy = Math.Abs(accuracy);
            if (accuracy < DoubleAccuracy) accuracy = DoubleAccuracy;
            var x2 = left;
            for (var x = left; x < right; x += accuracy)
            {
                x2 = x;
                var (y1, y2) = graph.GetY(x);
                if (!double.IsNaN(y1)) yield return new(x, y1);
                if (!double.IsNaN(y2) && y1 != y2) yield return new(x, y2);
            }

            if (x2 >= right) yield break;
            var (y3, y4) = graph.GetY(right);
            if (!double.IsNaN(y3)) yield return new(right, y3);
            if (!double.IsNaN(y4) && y3 != y4) yield return new(right, y4);
        }
    }
}
