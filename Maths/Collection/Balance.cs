using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Collection;

namespace Trivial.Maths;

public static partial class CollectionOperation
{
    /// <summary>
    /// Balances 2 collections.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static void Balance<T>(ref IEnumerable<T> a, ref IEnumerable<T> b, DifferentCollectionCountOptions options)
    {
        var aCol = a == null ? new List<T>() : new List<T>(a);
        var bCol = b == null ? new List<T>() : new List<T>(b);
        if (aCol.Count == bCol.Count) return;
        var compare = aCol.Count > bCol.Count;
        var shorter = compare ? bCol : aCol;
        var longer = compare ? aCol : bCol;
        var diff = compare ? aCol.Count - bCol.Count : bCol.Count - aCol.Count;
        switch (options)
        {
            case DifferentCollectionCountOptions.PadEnd:
                ListExtensions.PadEndTo(shorter, diff);
                if (compare) b = bCol;
                else a = aCol;
                return;
            case DifferentCollectionCountOptions.PadBegin:
                ListExtensions.PadBeginTo(shorter, diff);
                if (compare) b = bCol;
                else a = aCol;
                return;
            case DifferentCollectionCountOptions.TrimEnd:
                while (longer.Count > shorter.Count)
                {
                    longer.RemoveAt(shorter.Count);
                }

                if (compare) a = aCol;
                else b = bCol;
                return;
            case DifferentCollectionCountOptions.TrimBegin:
                while (longer.Count > shorter.Count)
                {
                    longer.RemoveAt(0);
                }

                if (compare) a = aCol;
                else b = bCol;
                return;
            case DifferentCollectionCountOptions.Abort:
                throw new InvalidOperationException("The count of 2 collections should be same.");
            default:
                throw new NotSupportedException("The options was not supported.", new ArgumentException("options is invalid.", nameof(options)));
        }
    }

    /// <summary>
    /// Balances 3 collections.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="options">The preprocessing option for calculating between 3 collections with different count.</param>
    /// <returns>A collection with items plused.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static void Balance<T>(ref IEnumerable<T> a, ref IEnumerable<T> b, ref IEnumerable<T> c, DifferentCollectionCountOptions options)
    {
        var aCol = a == null ? new List<T>() : new List<T>(a);
        var bCol = b == null ? new List<T>() : new List<T>(b);
        var cCol = c == null ? new List<T>() : new List<T>(c);
        if (aCol.Count == bCol.Count && aCol.Count == cCol.Count) return;
        var max = Math.Max(Math.Max(aCol.Count, bCol.Count), cCol.Count);
        var min = Math.Max(Math.Max(aCol.Count, bCol.Count), cCol.Count);
        switch (options)
        {
            case DifferentCollectionCountOptions.PadEnd:
                if (aCol.Count < max)
                {
                    ListExtensions.PadEndTo(aCol, max - aCol.Count);
                    a = aCol;
                }

                if (bCol.Count < max)
                {
                    ListExtensions.PadEndTo(bCol, max - bCol.Count);
                    b = bCol;
                }

                if (cCol.Count < max)
                {
                    ListExtensions.PadEndTo(cCol, max - cCol.Count);
                    c = cCol;
                }

                return;
            case DifferentCollectionCountOptions.PadBegin:
                if (aCol.Count < max)
                {
                    ListExtensions.PadBeginTo(aCol, max - aCol.Count);
                    a = aCol;
                }

                if (bCol.Count < max)
                {
                    ListExtensions.PadBeginTo(bCol, max - bCol.Count);
                    b = bCol;
                }

                if (cCol.Count < max)
                {
                    ListExtensions.PadBeginTo(cCol, max - cCol.Count);
                    c = cCol;
                }

                return;
            case DifferentCollectionCountOptions.TrimEnd:
                if (aCol.Count > min) a = aCol;
                while (aCol.Count > min)
                {
                    aCol.RemoveAt(min);
                }

                if (bCol.Count > min) b = bCol;
                while (bCol.Count > min)
                {
                    bCol.RemoveAt(min);
                }

                if (cCol.Count > min) c = cCol;
                while (cCol.Count > min)
                {
                    cCol.RemoveAt(min);
                }

                return;
            case DifferentCollectionCountOptions.TrimBegin:
                if (aCol.Count > min) a = aCol;
                while (aCol.Count > min)
                {
                    aCol.RemoveAt(0);
                }

                if (bCol.Count > min) b = bCol;
                while (bCol.Count > min)
                {
                    bCol.RemoveAt(0);
                }

                if (cCol.Count > min) c = cCol;
                while (cCol.Count > min)
                {
                    cCol.RemoveAt(0);
                }

                return;
            case DifferentCollectionCountOptions.Abort:
                throw new InvalidOperationException("The count of 3 collections should be same.");
            default:
                throw new NotSupportedException("The options was not supported.", new ArgumentException("options is invalid.", nameof(options)));
        }
    }

    /// <summary>
    /// Applies a specified function to the corresponding elements of 2 collections, producing a sequence of the results.
    /// </summary>
    /// <typeparam name="T1">The item type of the first collection.</typeparam>
    /// <typeparam name="T2">The item type of the second collection.</typeparam>
    /// <param name="a">The first collection.</param>
    /// <param name="b">The second collection.</param>
    /// <param name="options">The preprocessing option for calculating between 2 collections with different count.</param>
    /// <returns>A new collection that contains merged elements of 2 input collections.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<(T1, T2)> Zip<T1, T2>(IEnumerable<T1> a, IEnumerable<T2> b, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        var aCol = a == null ? new List<T1>() : new List<T1>(a);
        var bCol = b == null ? new List<T2>() : new List<T2>(b);
        if (aCol.Count == bCol.Count)
        {
            for (var i = 0; i < aCol.Count; i++)
            {
                yield return (aCol[i], bCol[i]);
            }

            yield break;
        }

        var max = Math.Max(aCol.Count, bCol.Count);
        var min = Math.Max(aCol.Count, bCol.Count);
        switch (options)
        {
            case DifferentCollectionCountOptions.PadEnd:
                for (var i = 0; i < max; i++)
                {
                    yield return (i < aCol.Count ? aCol[i] : default, i < bCol.Count ? bCol[i] : default);
                }

                break;
            case DifferentCollectionCountOptions.PadBegin:
                if (aCol.Count < max) ListExtensions.PadBeginTo(aCol, max - aCol.Count);
                if (bCol.Count < max) ListExtensions.PadBeginTo(bCol, max - bCol.Count);
                for (var i = 0; i < max; i++)
                {
                    yield return (aCol[i], bCol[i]);
                }

                break;
            case DifferentCollectionCountOptions.TrimEnd:
                for (var i = 0; i < min; i++)
                {
                    yield return (aCol[i], bCol[i]);
                }

                break;
            case DifferentCollectionCountOptions.TrimBegin:
                while (aCol.Count > min)
                {
                    aCol.RemoveAt(0);
                }

                while (bCol.Count > min)
                {
                    bCol.RemoveAt(0);
                }

                for (var i = 0; i < min; i++)
                {
                    yield return (aCol[i], bCol[i]);
                }

                break;
            case DifferentCollectionCountOptions.Abort:
                throw new InvalidOperationException("The count of 2 collections should be same.");
            default:
                throw new NotSupportedException("The options was not supported.", new ArgumentException("options is invalid.", nameof(options)));
        }
    }

    /// <summary>
    /// Applies a specified function to the corresponding elements of 3 collections, producing a sequence of the results.
    /// </summary>
    /// <typeparam name="T1">The item type of the first collection.</typeparam>
    /// <typeparam name="T2">The item type of the second collection.</typeparam>
    /// <typeparam name="T3">The item type of the third collection.</typeparam>
    /// <param name="a">The first collection.</param>
    /// <param name="b">The second collection.</param>
    /// <param name="c">The third collection.</param>
    /// <param name="options">The preprocessing option for calculating between 3 collections with different count.</param>
    /// <returns>A new collection that contains merged elements of 3 input collections.</returns>
    /// <exception cref="InvalidOperationException">The count of the collections are not same and the options is set to abort.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static IEnumerable<(T1, T2, T3)> Zip<T1, T2, T3>(IEnumerable<T1> a, IEnumerable<T2> b, IEnumerable<T3> c, DifferentCollectionCountOptions options = DifferentCollectionCountOptions.PadEnd)
    {
        var aCol = a == null ? new List<T1>() : new List<T1>(a);
        var bCol = b == null ? new List<T2>() : new List<T2>(b);
        var cCol = c == null ? new List<T3>() : new List<T3>(c);
        if (aCol.Count == bCol.Count && aCol.Count == cCol.Count)
        {
            for (var i = 0; i < aCol.Count; i++)
            {
                yield return (aCol[i], bCol[i], cCol[i]);
            }

            yield break;
        }

        var max = Math.Max(Math.Max(aCol.Count, bCol.Count), cCol.Count);
        var min = Math.Max(Math.Max(aCol.Count, bCol.Count), cCol.Count);
        switch (options)
        {
            case DifferentCollectionCountOptions.PadEnd:
                for (var i = 0; i < max; i++)
                {
                    yield return (i < aCol.Count ? aCol[i] : default, i < bCol.Count ? bCol[i] : default, i < cCol.Count ? cCol[i] : default);
                }

                break;
            case DifferentCollectionCountOptions.PadBegin:
                if (aCol.Count < max) ListExtensions.PadBeginTo(aCol, max - aCol.Count);
                if (bCol.Count < max) ListExtensions.PadBeginTo(bCol, max - bCol.Count);
                if (cCol.Count < max) ListExtensions.PadBeginTo(cCol, max - cCol.Count);
                for (var i = 0; i < max; i++)
                {
                    yield return (aCol[i], bCol[i], cCol[i]);
                }

                break;
            case DifferentCollectionCountOptions.TrimEnd:
                for (var i = 0; i < min; i++)
                {
                    yield return (aCol[i], bCol[i], cCol[i]);
                }

                break;
            case DifferentCollectionCountOptions.TrimBegin:
                while (aCol.Count > min)
                {
                    aCol.RemoveAt(0);
                }

                while (bCol.Count > min)
                {
                    bCol.RemoveAt(0);
                }

                while (cCol.Count > min)
                {
                    cCol.RemoveAt(0);
                }

                for (var i = 0; i < min; i++)
                {
                    yield return (aCol[i], bCol[i], cCol[i]);
                }

                break;
            case DifferentCollectionCountOptions.Abort:
                throw new InvalidOperationException("The count of 3 collections should be same.");
            default:
                throw new NotSupportedException("The options was not supported.", new ArgumentException("options is invalid.", nameof(options)));
        }
    }
}
