using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Trivial.Collection;

namespace Trivial.Maths;

/// <summary>
/// The preprocessing options for calculating between 2 collections with different count.
/// </summary>
public enum DifferentCollectionCountOptions : byte
{
    /// <summary>
    /// Pad end for shorter.
    /// </summary>
    PadEnd = 0,

    /// <summary>
    /// Pad begin for shorter.
    /// </summary>
    PadBegin = 1,

    /// <summary>
    /// Remove end for longer.
    /// </summary>
    TrimEnd = 2,

    /// <summary>
    /// Remove begin for longer.
    /// </summary>
    TrimBegin = 3,

    /// <summary>
    /// Abort and throw invalid operation exception.
    /// </summary>
    Abort = 4,
}

public static partial class CollectionOperation
{
    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<int> Plus(IEnumerable<int> a, IEnumerable<int> b, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Plus(a, b);
    }

    /// <summary>
    /// Pluses each item in 3 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<int> Plus(IEnumerable<int> a, IEnumerable<int> b, IEnumerable<int> c, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, ref c, options);
        return Arithmetic.Plus(a, b, c);
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<int> Plus(IEnumerable<int> a, IEnumerable<int> b, int c, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Plus(a, b, c);
    }

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items minused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<int> Minus(IEnumerable<int> a, IEnumerable<int> b, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Minus(a, b);
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<long> Plus(IEnumerable<long> a, IEnumerable<long> b, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Plus(a, b);
    }

    /// <summary>
    /// Pluses each item in 3 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<long> Plus(IEnumerable<long> a, IEnumerable<long> b, IEnumerable<long> c, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, ref c, options);
        return Arithmetic.Plus(a, b, c);
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<long> Plus(IEnumerable<long> a, IEnumerable<long> b, long c, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Plus(a, b, c);
    }

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items minused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<long> Minus(IEnumerable<long> a, IEnumerable<long> b, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Minus(a, b);
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<float> Plus(IEnumerable<float> a, IEnumerable<float> b, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Plus(a, b);
    }

    /// <summary>
    /// Pluses each item in 3 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<float> Plus(IEnumerable<float> a, IEnumerable<float> b, IEnumerable<float> c, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, ref c, options);
        return Arithmetic.Plus(a, b, c);
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<float> Plus(IEnumerable<float> a, IEnumerable<float> b, float c, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Plus(a, b, c);
    }

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items minused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<float> Minus(IEnumerable<float> a, IEnumerable<float> b, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Minus(a, b);
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<double> Plus(IEnumerable<double> a, IEnumerable<double> b, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Plus(a, b);
    }

    /// <summary>
    /// Pluses each item in 3 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<double> Plus(IEnumerable<double> a, IEnumerable<double> b, IEnumerable<double> c, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, ref c, options);
        return Arithmetic.Plus(a, b, c);
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<double> Plus(IEnumerable<double> a, IEnumerable<double> b, double c, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Plus(a, b, c);
    }

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items minused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<double> Minus(IEnumerable<double> a, IEnumerable<double> b, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Minus(a, b);
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<decimal> Plus(IEnumerable<decimal> a, IEnumerable<decimal> b, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Plus(a, b);
    }

    /// <summary>
    /// Pluses each item in 3 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<decimal> Plus(IEnumerable<decimal> a, IEnumerable<decimal> b, IEnumerable<decimal> c, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, ref c, options);
        return Arithmetic.Plus(a, b, c);
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<decimal> Plus(IEnumerable<decimal> a, IEnumerable<decimal> b, decimal c, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Plus(a, b, c);
    }

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items minused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<decimal> Minus(IEnumerable<decimal> a, IEnumerable<decimal> b, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Minus(a, b);
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<TimeSpan> Plus(IEnumerable<TimeSpan> a, IEnumerable<TimeSpan> b, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Plus(a, b);
    }

    /// <summary>
    /// Pluses each item in 3 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<TimeSpan> Plus(IEnumerable<TimeSpan> a, IEnumerable<TimeSpan> b, IEnumerable<TimeSpan> c, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, ref c, options);
        return Arithmetic.Plus(a, b, c);
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<TimeSpan> Plus(IEnumerable<TimeSpan> a, IEnumerable<TimeSpan> b, TimeSpan c, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Plus(a, b, c);
    }

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items minused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<TimeSpan> Minus(IEnumerable<TimeSpan> a, IEnumerable<TimeSpan> b, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        Balance(ref a, ref b, options);
        return Arithmetic.Minus(a, b);
    }
}
