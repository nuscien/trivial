using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Reflection;

/// <summary>
/// The version comparer.
/// </summary>
public class VersionComparer : IComparer<string>
{
    /// <summary>
    /// Gets a value indicating whether x is wide version scope.
    /// </summary>
    public bool IsWideX { get; set; }

    /// <summary>
    /// Compares two versions and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="x">The first version to compare.</param>
    /// <param name="y">The second version to compare.</param>
    /// <returns>
    /// <para>
    /// A signed integer that indicates the relative values of x and y, as shown in the following.
    /// </para>
    /// <list type="bullet">
    /// <item>Value Meaning Less than zero x is less than y.</item>
    /// <item>Zero x equals y.</item>
    /// <item>Greater than zero x is greater than y.</item>
    /// </list>
    /// </returns>
    public int Compare(string x, string y)
    {
        return Compare(x, y, IsWideX);
    }

    /// <summary>
    /// Converts the item to version.
    /// </summary>
    /// <param name="s">the version string.</param>
    /// <returns>A version instance.</returns>
    public static Version ToVersion(string s)
    {
#pragma warning disable IDE0057
        s = s?.Trim();
        if (string.IsNullOrEmpty(s)) return null;
        var end = s.IndexOf('-');
        if (end >= 0) s = s.Substring(0, end);
        end = s.IndexOf("+");
        if (end >= 0) s = s.Substring(0, end);
        while (s.EndsWith(".*")) s = s.Substring(0, s.Length - 2);
        if (s.Length == 0 || s == "*") return null;
        var arr = s.Split('.');
        if (!int.TryParse(arr[0], out var major)) return null;
        if (arr.Length < 2 || !int.TryParse(arr[1], out var minor)) return new Version(major, 0);
        if (arr.Length < 3 || !int.TryParse(arr[2], out var build)) return new Version(major, minor);
        if (arr.Length < 4 || !int.TryParse(arr[3], out var rev)) return new Version(major, minor, build);
        return new Version(major, minor, build, rev);
#pragma warning restore IDE0057
    }

    /// <summary>
    /// Compares two versions and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="x">The first version to compare.</param>
    /// <param name="y">The second version to compare.</param>
    /// <param name="isWideX">true if x is wide version scope; otherwise, false.</param>
    /// <returns>
    /// <para>
    /// A signed integer that indicates the relative values of x and y, as shown in the following.
    /// </para>
    /// <list type="bullet">
    /// <item>Value Meaning Less than zero x is less than y.</item>
    /// <item>Zero x equals y.</item>
    /// <item>Greater than zero x is greater than y.</item>
    /// </list>
    /// </returns>
    public static int Compare(string x, string y, bool isWideX)
    {
#pragma warning disable IDE0057
        x = x?.Trim();
        y = y?.Trim();
        if (x == y) return 0;
        var r = 1;
        if (string.IsNullOrEmpty(y) || y.StartsWith("+")) return r;
        if (string.IsNullOrEmpty(x)) return 0;
        if (x.StartsWith("+")) return -r;
        while (x.EndsWith(".*"))
        {
            x = x.Substring(0, x.Length - 2);
            isWideX = true;
        }

        if (x.Length == 0 || x == "*") return 0;
        var leftMetaPos = x.IndexOf("+");
        if (leftMetaPos > 0) x = x.Substring(0, leftMetaPos);
        var rightMetaPos = y.IndexOf("+");
        if (rightMetaPos > 0) y = y.Substring(0, rightMetaPos);
        var leftArr = x.Split('-');
        var rightArr = y.Split('-');
        var arrLen = Math.Min(leftArr.Length, rightArr.Length);
        for (var j = 0; j < arrLen; j++)
        {
            var a = leftArr[j].Split('.');
            var b = rightArr[j].Split('.');
            var len = Math.Min(a.Length, b.Length);
            for (var i = 0; i < len; i++)
            {
                try
                {
                    r++;
                    var aPart = a[i]?.Trim();
                    if (string.IsNullOrEmpty(aPart)) aPart = "0";
                    var bPart = b[i]?.Trim();
                    if (string.IsNullOrEmpty(bPart)) bPart = "0";
                    if (aPart == bPart) continue;
                    if (int.TryParse(aPart, out var aNum) && int.TryParse(bPart, out var bNum))
                    {
                        if (aNum == bNum) continue;
                        return aNum < bNum ? -r : r;
                    }

                    return aPart.CompareTo(bPart) < 0 ? -r : r;
                }
                catch (InvalidOperationException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (NullReferenceException)
                {
                }
                catch (NotSupportedException)
                {
                }
            }

            if (isWideX && a.Length <= b.Length && j == arrLen - 1) return 0;
            if (a.Length != b.Length) return a.Length < b.Length ? -r : r;
        }

        if (leftArr.Length == rightArr.Length) return 0;
        return leftArr.Length > rightArr.Length ? -r : r;
#pragma warning restore IDE0057
    }
}
