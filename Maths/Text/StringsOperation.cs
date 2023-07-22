using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Text;

namespace Trivial.Maths;

public static partial class CollectionOperation
{
    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static string Merge(StringCollectionOperators op, IEnumerable<string> input)
    {
        if (input == null) return null;
        if (input is string[] arr)
        {
            if (arr.Length < 1) return null;
        }
        else if (input is List<string> col)
        {
            if (col.Count < 1) return null;
        }
        else
        {
            col = input?.ToList();
            input = col;
            if (col.Count < 1) return null;
        }

        switch (op)
        {
            case StringCollectionOperators.Empty:
                return string.Empty;
            case StringCollectionOperators.Join:
                return string.Join(string.Empty, input);
            case StringCollectionOperators.Lines:
                return string.Join(Environment.NewLine, input);
            case StringCollectionOperators.Tabs:
                return Join('\t', input);
            case StringCollectionOperators.Tags:
                return Join(';', input);
            case StringCollectionOperators.Commas:
                return Join(',', input);
            case StringCollectionOperators.Dots:
                return Join('.', input);
            case StringCollectionOperators.Slashes:
                return Join('/', input);
            case StringCollectionOperators.VerticalLines:
                return Join('|', input);
            case StringCollectionOperators.WhiteSpaces:
                return Join(' ', input);
            case StringCollectionOperators.DoubleWhiteSpaces:
                return string.Join("  ", input);
            case StringCollectionOperators.TripleWhiteSpaces:
                return string.Join("   ", input);
            case StringCollectionOperators.And:
                return string.Join(" & ", input);
            case StringCollectionOperators.SplitPoints:
                return string.Join(" · ", input);
            case StringCollectionOperators.JsonArray:
                {
                    var json = new JsonArrayNode();
                    json.AddRange(input);
                    return json.ToString();
                }
            case StringCollectionOperators.Bullet:
                input = input.Select(ele => string.Concat("· ", ele));
                return string.Join(Environment.NewLine, input);
            case StringCollectionOperators.Numbering:
                {
                    input = input.Select((ele, i) => string.Concat(i, ' ', '\t', ele));
                    return string.Join(Environment.NewLine, input);
                }
            case StringCollectionOperators.First:
                return input.First();
            case StringCollectionOperators.Last:
                return input.Last();
            case StringCollectionOperators.Longest:
                {
                    int i = 0;
                    string s = null;
                    foreach (var item in input)
                    {
                        if (item == null || item.Length < i) continue;
                        if (item.Length == i)
                        {
                            if (i == 0) s = string.Empty;
                            continue;
                        }

                        s = item;
                        i = item.Length;
                    }

                    return s;
                }
            case StringCollectionOperators.LastLongest:
                {
                    int i = 0;
                    string s = null;
                    foreach (var item in input)
                    {
                        if (item == null || item.Length < i) continue;
                        if (i == 0) s = string.Empty;
                        s = item;
                        i = item.Length;
                    }

                    return s;
                }
            case StringCollectionOperators.Shortest:
                {
                    int i = -1;
                    string s = null;
                    foreach (var item in input)
                    {
                        if (item == null) return null;
                        if (i >= 0 && item.Length >= i) continue;
                        s = item;
                        i = item.Length;
                    }

                    return s;
                }
            case StringCollectionOperators.LastShortest:
                {
                    int i = -1;
                    string s = null;
                    foreach (var item in input)
                    {
                        if (item == null) return null;
                        if (i >= 0 && item.Length > i) continue;
                        s = item;
                        i = item.Length;
                    }

                    return s;
                }
            default:
                throw NotSupported(op);
        }
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static string Merge(StringCollectionOperators op, params string[] input)
        => Merge(op, input as IEnumerable<string>);

    private static string Join(char seperator, IEnumerable<string> input)
#if NET6_0_OR_GREATER
        => string.Join(seperator, input);
#else
        => string.Join(seperator.ToString(), input);
#endif
}
