using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Tasks;

namespace Trivial.Collection;

/// <summary>
/// The list utility.
/// </summary>
public static partial class ListExtensions
{
    /// <summary>
    /// Creates a range of number.
    /// </summary>
    /// <param name="start">A number to start.</param>
    /// <param name="count">The length.</param>
    /// <param name="step">The optional step.</param>
    /// <returns>A list.</returns>
    public static IEnumerable<int> CreateNumberRange(int start, int count, int step = 1)
    {
        if (step == 1)
        {
            for (var i = 0; i < count; i++)
            {
                yield return i + start;
            }
        }
        else if (step == 0)
        {
            for (var i = 0; i < count; i++)
            {
                yield return start;
            }
        }
        else
        {
            for (var i = 0; i < count; i++)
            {
                yield return i * step + start;
            }
        }
    }

    /// <summary>
    /// Creates a range of number.
    /// </summary>
    /// <param name="start">A number to start.</param>
    /// <param name="count">The length.</param>
    /// <param name="step">The optional step.</param>
    /// <returns>A list.</returns>
    public static IEnumerable<uint> CreateNumberRange(uint start, int count, uint step = 1)
    {
        if (step == 1)
        {
            for (uint i = 0; i < count; i++)
            {
                yield return i + start;
            }
        }
        else if (step == 0)
        {
            for (uint i = 0; i < count; i++)
            {
                yield return start;
            }
        }
        else
        {
            for (uint i = 0; i < count; i++)
            {
                yield return i * step + start;
            }
        }
    }

    /// <summary>
    /// Creates a range of number.
    /// </summary>
    /// <param name="start">A number to start.</param>
    /// <param name="count">The length.</param>
    /// <param name="step">The optional step.</param>
    /// <returns>A list.</returns>
    public static IEnumerable<long> CreateNumberRange(long start, int count, long step = 1)
    {
        if (step == 1)
        {
            for (var i = 0L; i < count; i++)
            {
                yield return i + start;
            }
        }
        else if (step == 0)
        {
            for (var i = 0; i < count; i++)
            {
                yield return start;
            }
        }
        else
        {
            for (var i = 0L; i < count; i++)
            {
                yield return i * step + start;
            }
        }
    }

    /// <summary>
    /// Creates a range of number.
    /// </summary>
    /// <param name="start">A number to start.</param>
    /// <param name="count">The length.</param>
    /// <param name="step">The optional step.</param>
    /// <returns>A list.</returns>
    public static IEnumerable<float> CreateNumberRange(float start, int count, float step = 1)
    {
        if (step == 1)
        {
            for (var i = 0; i < count; i++)
            {
                yield return i + start;
            }
        }
        else if (step == 0)
        {
            for (var i = 0; i < count; i++)
            {
                yield return start;
            }
        }
        else
        {
            for (var i = 0; i < count; i++)
            {
                yield return i * step + start;
            }
        }
    }

    /// <summary>
    /// Creates a range of number.
    /// </summary>
    /// <param name="start">A number to start.</param>
    /// <param name="count">The length.</param>
    /// <param name="step">The optional step.</param>
    /// <returns>A list.</returns>
    public static IEnumerable<double> CreateNumberRange(double start, int count, double step = 1)
    {
        if (step == 1)
        {
            for (var i = 0; i < count; i++)
            {
                yield return i + start;
            }
        }
        else if (step == 0)
        {
            for (var i = 0; i < count; i++)
            {
                yield return start;
            }
        }
        else
        {
            for (var i = 0; i < count; i++)
            {
                yield return i * step + start;
            }
        }
    }

    /// <summary>
    /// Creates a range of number.
    /// </summary>
    /// <param name="start">A number to start.</param>
    /// <param name="count">The length.</param>
    /// <param name="step">The optional step.</param>
    /// <returns>A list.</returns>
    public static IEnumerable<decimal> CreateNumberRange(decimal start, decimal count, decimal step = 1)
    {
        if (step == 1)
        {
            for (var i = 0; i < count; i++)
            {
                yield return i + start;
            }
        }
        else if (step == 0)
        {
            for (var i = 0; i < count; i++)
            {
                yield return start;
            }
        }
        else
        {
            for (var i = 0; i < count; i++)
            {
                yield return i * step + start;
            }
        }
    }

    /// <summary>
    /// Creates a collection with a specific number of item.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="count">The count.</param>
    /// <param name="item">The default item.</param>
    /// <returns>A sequence.</returns>
    public static IEnumerable<T> Create<T>(int count, T item = default)
    {
        for (var i = 0; i < count; i++)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Creates an array with a specific number of item.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="count">The count.</param>
    /// <param name="item">The default item.</param>
    /// <returns>An array.</returns>
    public static T[] CreateArray<T>(int count, T item = default)
    {
        var arr = new T[count];
        for (var i = 0; i < count; i++)
        {
            arr[i] = item;
        }

        return arr;
    }

    /// <summary>
    /// Creates a range of number with duration.
    /// </summary>
    /// <param name="duration">The duration between each number.</param>
    /// <param name="count">The length.</param>
    /// <returns>A sequence.</returns>
    /// <exception cref="ArgumentOutOfRangeException">duration should be greater than zero millisecond (0ms).</exception>
    public static async IAsyncEnumerable<int> CreateNumberRangeAsync(TimeSpan duration, int count)
    {
        if (duration.TotalMilliseconds < 0) throw new ArgumentOutOfRangeException(nameof(duration), "duration should be greater than zero millisecond (0ms).");
        for (var i = 0; i < count; i++)
        {
            await Task.Delay(duration);
            yield return i;
        }
    }

    /// <summary>
    /// Creates a range of number with duration.
    /// </summary>
    /// <param name="policy">The policy to sleep between each number.</param>
    /// <returns>A sequence.</returns>
    public static async IAsyncEnumerable<int> CreateNumberRangeAsync(IRetryPolicy policy)
    {
        var instance = policy?.CreateInstance();
        if (instance == null) yield break;
        var i = 0;
        while (true)
        {
            var next = instance.Next();
            if (!next.HasValue) yield break;
            if (next.Value.TotalMilliseconds > 0) await Task.Delay(next.Value);
            yield return i;
            i++;
        }
    }
}
