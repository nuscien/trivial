using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Maths;

/// <summary>
/// The interface for outline as point collection.
/// </summary>
public interface IPixelOutline<TUnit> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
{
    /// <summary>
    /// Generates point collection in the specific zone and accuracy.
    /// </summary>
    /// <param name="left">The left boundary.</param>
    /// <param name="right">The right boundary.</param>
    /// <param name="accuracy">The step in x.</param>
    /// <returns>A point collection.</returns>
    IEnumerable<Point2D<TUnit>> DrawPoints(TUnit left, TUnit right, TUnit accuracy);
}

/// <summary>
/// The interface for outline as point collection.
/// </summary>
public interface ICoordinateSinglePoint<TUnit> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
{
    /// <summary>
    /// Gets y by x.
    /// </summary>
    /// <param name="x">X.</param>
    /// <returns>Y.</returns>
    TUnit GetY(TUnit x);

    /// <summary>
    /// Gets x by y.
    /// </summary>
    /// <param name="y">Y.</param>
    /// <returns>X.</returns>
    TUnit GetX(TUnit y);

    /// <summary>
    /// Test if a point is on the line.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>true if the point is on the line; otherwise, false.</returns>
    bool Contains(Point2D<TUnit> point);
}

/// <summary>
/// The interface for outline as point collection.
/// </summary>
public interface ICoordinateTuplePoint<TUnit> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
{
    /// <summary>
    /// Gets y by x.
    /// </summary>
    /// <param name="x">X.</param>
    /// <returns>Y.</returns>
    (TUnit, TUnit) GetY(TUnit x);

    /// <summary>
    /// Gets x by y.
    /// </summary>
    /// <param name="y">Y.</param>
    /// <returns>X.</returns>
    (TUnit, TUnit) GetX(TUnit y);

    /// <summary>
    /// Test if a point is on the line.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>true if the point is on the line; otherwise, false.</returns>
    bool Contains(Point2D<TUnit> point);
}
