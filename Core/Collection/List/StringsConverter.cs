using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Text;

namespace Trivial.Collection;

public static partial class ListExtensions
{
    /// <summary>
    /// Generates a string collection.
    /// </summary>
    /// <param name="count">The count.</param>
    /// <param name="value">The default value to fill.</param>
    /// <returns>A new string list.</returns>
    public static List<string> ToStringCollection(int count, string value = null)
    {
        var arr = new List<string>();
        for (int i = 0; i < count; i++)
        {
            arr.Add(value);
        }

        return arr;
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>A new string list.</returns>
    public static IEnumerable<string> ToStringCollection(this IEnumerable<JsonStringNode> input)
    {
        foreach (var item in input)
        {
            yield return item?.StringValue;
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static IEnumerable<string> ToStringCollection(this IEnumerable<bool> input, IFormatProvider provider = null)
    {
        if (input == null) yield break;
        foreach (var item in input)
        {
            yield return item.ToString(provider);
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static string[] ToStringArray(this bool[] input, IFormatProvider provider = null)
    {
        if (input == null) return null;
        var arr = new string[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            arr[i] = input[i].ToString(provider);
        }

        return arr;
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static IEnumerable<string> ToStringCollection(this IEnumerable<int> input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) yield break;
        foreach (var item in input)
        {
            yield return item.ToString(format, provider);
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static string[] ToStringArray(this int[] input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) return null;
        var arr = new string[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            arr[i] = input[i].ToString(format, provider);
        }

        return arr;
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static IEnumerable<string> ToStringCollection(this IEnumerable<long> input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) yield break;
        foreach (var item in input)
        {
            yield return item.ToString(format, provider);
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static string[] ToStringArray(this long[] input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) return null;
        var arr = new string[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            arr[i] = input[i].ToString(format, provider);
        }

        return arr;
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static IEnumerable<string> ToStringCollection(this IEnumerable<float> input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) yield break;
        foreach (var item in input)
        {
            yield return item.ToString(format, provider);
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static string[] ToStringArray(this float[] input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) return null;
        var arr = new string[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            arr[i] = input[i].ToString(format, provider);
        }

        return arr;
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static IEnumerable<string> ToStringCollection(this IEnumerable<double> input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) yield break;
        foreach (var item in input)
        {
            yield return item.ToString(format, provider);
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static string[] ToStringArray(this double[] input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) return null;
        var arr = new string[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            arr[i] = input[i].ToString(format, provider);
        }

        return arr;
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static IEnumerable<string> ToStringCollection(this IEnumerable<decimal> input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) yield break;
        foreach (var item in input)
        {
            yield return item.ToString(format, provider);
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static string[] ToStringArray(this decimal[] input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) return null;
        var arr = new string[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            arr[i] = input[i].ToString(format, provider);
        }

        return arr;
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <returns>The string collection converted.</returns>
    public static IEnumerable<string> ToStringCollection(this IEnumerable<Guid> input, string format = null)
    {
        if (input == null) yield break;
        foreach (var item in input)
        {
            yield return item.ToString(format);
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <returns>The string collection converted.</returns>
    public static string[] ToStringArray(this Guid[] input, string format = null)
    {
        if (input == null) return null;
        var arr = new string[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            arr[i] = input[i].ToString(format);
        }

        return arr;
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static IEnumerable<string> ToStringCollection(this IEnumerable<TimeSpan> input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) yield break;
        foreach (var item in input)
        {
            yield return item.ToString(format, provider);
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static string[] ToStringArray(this TimeSpan[] input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) return null;
        var arr = new string[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            arr[i] = input[i].ToString(format, provider);
        }

        return arr;
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static IEnumerable<string> ToStringCollection(this IEnumerable<DateTime> input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) yield break;
        foreach (var item in input)
        {
            yield return item.ToString(format, provider);
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static string[] ToStringArray(this DateTime[] input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) return null;
        var arr = new string[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            arr[i] = input[i].ToString(format, provider);
        }

        return arr;
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static IEnumerable<string> ToStringCollection(this IEnumerable<DateTimeOffset> input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) yield break;
        foreach (var item in input)
        {
            yield return item.ToString(format, provider);
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static string[] ToStringArray(this DateTimeOffset[] input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) return null;
        var arr = new string[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            arr[i] = input[i].ToString(format, provider);
        }

        return arr;
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static IEnumerable<string> ToStringCollection(this IEnumerable<DateOnly> input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) yield break;
        foreach (var item in input)
        {
            yield return item.ToString(format, provider);
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static string[] ToStringArray(this DateOnly[] input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) return null;
        var arr = new string[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            arr[i] = input[i].ToString(format, provider);
        }

        return arr;
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static IEnumerable<string> ToStringCollection(this IEnumerable<TimeOnly> input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) yield break;
        foreach (var item in input)
        {
            yield return item.ToString(format, provider);
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The string collection converted.</returns>
    public static string[] ToStringArray(this TimeOnly[] input, string format = null, IFormatProvider provider = null)
    {
        if (input == null) return null;
        var arr = new string[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            arr[i] = input[i].ToString(format, provider);
        }
        
        return arr;
    }
#endif
}
