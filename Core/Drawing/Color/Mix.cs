using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Drawing;

/// <summary>
/// The types to mix colors.
/// </summary>
public enum ColorMixTypes : byte
{
    /// <summary>
    /// Average (mean).
    /// Like 2 pigments mix together.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// The layer of the blend color covers the layer of the base color.
    /// </summary>
    Cover = 1,

    /// <summary>
    /// Merge each channel by the maximum value.
    /// Like 2 lights shine the same place.
    /// </summary>
    Lighten = 2,

    /// <summary>
    /// Merge each channel by the minimum value.
    /// Like 2 optical filters overlap.
    /// </summary>
    Darken = 3,

    /// <summary>
    /// Merge each channel by the minimum value or maximum value.
    /// So the saturation value of the new color will be as higher as the one merged.
    /// </summary>
    Wetness = 4,

    /// <summary>
    /// Merge each channel by the middle value.
    /// So the saturation value of the new color will be as lower as the one merged.
    /// </summary>
    Dryness = 5,

    /// <summary>
    /// Color linear dodge.
    /// Like 2 lights increase each other.
    /// </summary>
    Weaken = 6,

    /// <summary>
    /// Color linear burn.
    /// Like 2 optical filters overlap with additional loss.
    /// </summary>
    Deepen = 7,

    /// <summary>
    /// Emphasize each channel.
    /// Color dodge if the channel in base color is greater than gray; otherwise, color burn.
    /// </summary>
    Emphasis = 8,

    /// <summary>
    /// Add each channel of the blend color and the base color. Then cover to fit.
    /// </summary>
    Accent = 9,

    /// <summary>
    /// Add each channel of the blend color and the base color. Then contain to fit.
    /// </summary>
    Add = 10,

    /// <summary>
    /// Remove each channel value of the blend color by the base color.
    /// </summary>
    Remove = 11,

    /// <summary>
    /// Diff absolutely each channel of the blend color by the base color.
    /// </summary>
    Diff = 12,

    /// <summary>
    /// Diff cycled each channel of the blend color by the base color.
    /// </summary>
    Distance = 13,

    /// <summary>
    /// Symmetry each channel of the blend color by the base color.
    /// </summary>
    Symmetry = 14,

    /// <summary>
    /// Translate each channel of the blend color away from the base color with same gap and cover to fit.
    /// </summary>
    Strengthen = 15,
}

/// <summary>
/// Color channels.
/// </summary>
[Flags]
public enum ColorChannels : byte
{
    /// <summary>
    /// Red.
    /// </summary>
    Red = 1,

    /// <summary>
    /// Green.
    /// </summary>
    Green = 2,

    /// <summary>
    /// Blue.
    /// </summary>
    Blue = 4
}

/// <summary>
/// Color calculator.
/// </summary>
public static partial class ColorCalculator
{
    /// <summary>
    /// Adds a color on another one.
    /// </summary>
    /// <param name="top">The color on top.</param>
    /// <param name="bottom">The color on bottom.</param>
    /// <returns>A new color.</returns>
    public static Color Overlay(Color top, Color bottom)
    {
        if (top.A == 0) return bottom;
        if (bottom.A == 0 || top.A == 255) return top;
        var ratio = top.A * 1f / 255;
        if (bottom.A < 255) ratio += (255 - bottom.A) / 255f * (1 - ratio);
        if (ratio >= 1) return top;
        var negRatio = 1 - ratio;
#if NETFRAMEWORK
        var a = 255 - (int)Math.Round((255d - top.A) * (255d - bottom.A) * 255);
#else
        var a = 255 - (int)MathF.Round((255f - top.A) * (255f - bottom.A) * 255);
#endif
        return Color.FromArgb(
            a,
            ToChannel(top.R * ratio + bottom.R * negRatio),
            ToChannel(top.G * ratio + bottom.G * negRatio),
            ToChannel(top.B * ratio + bottom.B * negRatio));
    }

    /// <summary>
    /// Adds a color on another one.
    /// </summary>
    /// <param name="aA">The alpha channel of blend color.</param>
    /// <param name="aR">The red channel of blend color.</param>
    /// <param name="aG">The green channel of blend color.</param>
    /// <param name="aB">The blue channel of blend color.</param>
    /// <param name="bA">The alpha channel of base color.</param>
    /// <param name="bR">The red channel of base color.</param>
    /// <param name="bG">The green channel of base color.</param>
    /// <param name="bB">The blue channel of base color.</param>
    /// <returns>A new color.</returns>
    public static Color Overlay(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
    {
        if (aA == 0) return Color.FromArgb(bA, bR, bG, bB);
        if (bA == 0 || aA == 255) return Color.FromArgb(aA, aR, aG, aB);
        var ratio = aA * 1f / 255;
        if (bA < 255) ratio += (255 - bA) / 255f * (1 - ratio);
        if (ratio >= 1) return Color.FromArgb(aA, aR, aG, aB);
        var negRatio = 1 - ratio;
#if NETFRAMEWORK
        var a = 255 - (int)Math.Round((255d - aA) * (255d - bA) * 255);
#else
        var a = 255 - (int)MathF.Round((255f - aA) * (255f - bA) * 255);
#endif
        return Color.FromArgb(
            a,
            ToChannel(aR * ratio + bR * negRatio),
            ToChannel(aG * ratio + bG * negRatio),
            ToChannel(aB * ratio + bB * negRatio));
    }

    /// <summary>
    /// Adds a color on another one.
    /// </summary>
    /// <param name="top">The color on top.</param>
    /// <param name="alpha">Alpha for top color. Value is from 0 to 1.</param>
    /// <param name="bottom">The color on bottom.</param>
    /// <returns>A new color.</returns>
    public static Color Overlay(Color top, double alpha, Color bottom)
        => Overlay(Opacity(top, alpha), bottom);

    /// <summary>
    /// Adds a color on another one.
    /// </summary>
    /// <param name="top">The color on top.</param>
    /// <param name="alpha">Alpha for top color. Value is from 0 to 1.</param>
    /// <param name="bottom">The color on bottom.</param>
    /// <returns>A new color.</returns>
    public static Color Overlay(Color top, float alpha, Color bottom)
        => Overlay(Opacity(top, alpha), bottom);

    /// <summary>
    /// Adds a color on another one.
    /// </summary>
    /// <param name="top">The color on top.</param>
    /// <param name="alpha">Alpha for top color. Value is from 0 to 1.</param>
    /// <param name="bottom">The color on bottom.</param>
    /// <returns>A new color.</returns>
    public static Color Overlay(Color top, byte alpha, Color bottom)
        => Overlay(Opacity(top, alpha), bottom);

    /// <summary>
    /// Adds a color on another one.
    /// </summary>
    /// <param name="top">The color on top.</param>
    /// <param name="middle">The color in middle.</param>
    /// <param name="bottom">The color on bottom.</param>
    /// <returns>A new color.</returns>
    public static Color Overlay(Color top, Color middle, Color bottom)
        => Overlay(top, Overlay(middle, bottom));

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="type">The type to mix colors.</param>
    /// <param name="a">The blend color.</param>
    /// <param name="b">The base color.</param>
    /// <returns>A new color mixed.</returns>
    public static Color Mix(ColorMixTypes type, Color a, Color b)
        => type switch
        {
            ColorMixTypes.Cover => MixByCover(a, b),
            ColorMixTypes.Lighten => MixByLighten(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B),
            ColorMixTypes.Darken => MixByDarken(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B),
            ColorMixTypes.Weaken => MixByWeaken(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B),
            ColorMixTypes.Wetness => MixByWetness(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B),
            ColorMixTypes.Dryness => MixByDryness(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B),
            ColorMixTypes.Deepen => MixByDeepen(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B),
            ColorMixTypes.Emphasis => MixByEmphasis(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B),
            ColorMixTypes.Accent => MixByAccent(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B),
            ColorMixTypes.Add => MixByAdd(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B),
            ColorMixTypes.Remove => MixByRemove(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B),
            ColorMixTypes.Diff => MixByDiff(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B),
            ColorMixTypes.Distance => MixByDistance(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B),
            ColorMixTypes.Symmetry => MixBySymmetry(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B),
            ColorMixTypes.Strengthen => MixByStrengthen(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B),
            _ => MixByMean(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B)
        };

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="type">The type to mix colors.</param>
    /// <param name="aA">The alpha channel of blend color.</param>
    /// <param name="aR">The red channel of blend color.</param>
    /// <param name="aG">The green channel of blend color.</param>
    /// <param name="aB">The blue channel of blend color.</param>
    /// <param name="bA">The alpha channel of base color.</param>
    /// <param name="bR">The red channel of base color.</param>
    /// <param name="bG">The green channel of base color.</param>
    /// <param name="bB">The blue channel of base color.</param>
    /// <returns>A new color mixed.</returns>
    public static Color Mix(ColorMixTypes type, byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
        => type switch
        {
            ColorMixTypes.Cover => MixByCover(aA, aR, aG, aB, bA, bR, bG, bB),
            ColorMixTypes.Lighten => MixByLighten(aA, aR, aG, aB, bA, bR, bG, bB),
            ColorMixTypes.Darken => MixByDarken(aA, aR, aG, aB, bA, bR, bG, bB),
            ColorMixTypes.Weaken => MixByWeaken(aA, aR, aG, aB, bA, bR, bG, bB),
            ColorMixTypes.Wetness => MixByWetness(aA, aR, aG, aB, bA, bR, bG, bB),
            ColorMixTypes.Dryness => MixByDryness(aA, aR, aG, aB, bA, bR, bG, bB),
            ColorMixTypes.Deepen => MixByDeepen(aA, aR, aG, aB, bA, bR, bG, bB),
            ColorMixTypes.Emphasis => MixByEmphasis(aA, aR, aG, aB, bA, bR, bG, bB),
            ColorMixTypes.Accent => MixByAccent(aA, aR, aG, aB, bA, bR, bG, bB),
            ColorMixTypes.Add => MixByAdd(aA, aR, aG, aB, bA, bR, bG, bB),
            ColorMixTypes.Remove => MixByRemove(aA, aR, aG, aB, bA, bR, bG, bB),
            ColorMixTypes.Diff => MixByDiff(aA, aR, aG, aB, bA, bR, bG, bB),
            ColorMixTypes.Distance => MixByDistance(aA, aR, aG, aB, bA, bR, bG, bB),
            ColorMixTypes.Symmetry => MixBySymmetry(aA, aR, aG, aB, bA, bR, bG, bB),
            ColorMixTypes.Strengthen => MixByStrengthen(aA, aR, aG, aB, bA, bR, bG, bB),
            _ => MixByMean(aA, aR, aG, aB, bA, bR, bG, bB)
        };

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="level">The relative saturation level.</param>
    /// <param name="a">The blend color.</param>
    /// <param name="b">The base color.</param>
    /// <returns>A new color mixed.</returns>
    public static Color Mix(RelativeSaturationLevels level, Color a, Color b)
        => Saturate(MixByMean(a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B), level);

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="level">The relative saturation level.</param>
    /// <param name="aA">The alpha channel of blend color.</param>
    /// <param name="aR">The red channel of blend color.</param>
    /// <param name="aG">The green channel of blend color.</param>
    /// <param name="aB">The blue channel of blend color.</param>
    /// <param name="bA">The alpha channel of base color.</param>
    /// <param name="bR">The red channel of base color.</param>
    /// <param name="bG">The green channel of base color.</param>
    /// <param name="bB">The blue channel of base color.</param>
    /// <returns>A new color mixed.</returns>
    public static Color Mix(RelativeSaturationLevels level, byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
        => Saturate(MixByMean(aA, aR, aG, aB, bA, bR, bG, bB), level);


    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="merge">The handler to merge each channel.</param>
    /// <param name="a">The blend color.</param>
    /// <param name="b">The base color.</param>
    /// <returns>A new color mixed.</returns>
    public static Color Mix(Func<byte, byte, ColorChannels, byte> merge, Color a, Color b)
        => Mix(merge, a.A, a.R, a.G, a.B, b.A, b.R, b.G, b.B);

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="merge">The handler to merge each channel.</param>
    /// <param name="aA">The alpha channel of blend color.</param>
    /// <param name="aR">The red channel of blend color.</param>
    /// <param name="aG">The green channel of blend color.</param>
    /// <param name="aB">The blue channel of blend color.</param>
    /// <param name="bA">The alpha channel of base color.</param>
    /// <param name="bR">The red channel of base color.</param>
    /// <param name="bG">The green channel of base color.</param>
    /// <param name="bB">The blue channel of base color.</param>
    /// <returns>A new color mixed.</returns>
    public static Color Mix(Func<byte, byte, ColorChannels, byte> merge, byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
    {
        if (merge == null) return MixByMean(aA, aR, aG, aB, bA, bR, bG, bB);
        var red = merge(aR, bR, ColorChannels.Red);
        var green = merge(aG, bG, ColorChannels.Green);
        var blue = merge(aB, bB, ColorChannels.Blue);
        return MixWithAlpha(red, green, blue, aA, aR, aG, aB, bA, bR, bG, bB);
    }

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="type">The type to mix colors.</param>
    /// <param name="a">The collection of blend color.</param>
    /// <param name="b">The collection of base color.</param>
    /// <returns>A new color collection mixed.</returns>
    public static IEnumerable<Color> Mix(ColorMixTypes type, IEnumerable<Color> a, IEnumerable<Color> b)
        => Mix(Mix, type, a, b);

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="level">The relative saturation level.</param>
    /// <param name="a">The collection of blend color.</param>
    /// <param name="b">The collection of base color.</param>
    /// <returns>A new color collection mixed.</returns>
    public static IEnumerable<Color> Mix(RelativeSaturationLevels level, IEnumerable<Color> a, IEnumerable<Color> b)
        => Mix(Mix, level, a, b);

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="merge">The handler to merge each channel.</param>
    /// <param name="a">The blend color.</param>
    /// <param name="b">The base color.</param>
    /// <returns>A new color mixed.</returns>
    public static IEnumerable<Color> Mix(Func<byte, byte, ColorChannels, byte> merge, IEnumerable<Color> a, IEnumerable<Color> b)
        => Mix(Mix, merge, a, b);

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="type">The type to mix colors.</param>
    /// <param name="a">The collection of blend color.</param>
    /// <param name="b">The collection of base color.</param>
    /// <returns>A new color collection mixed.</returns>
    public static Color[] Mix(ColorMixTypes type, ReadOnlySpan<Color> a, ReadOnlySpan<Color> b)
        => Mix(Mix, type, a, b);

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="level">The relative saturation level.</param>
    /// <param name="a">The collection of blend color.</param>
    /// <param name="b">The collection of base color.</param>
    /// <returns>A new color collection mixed.</returns>
    public static Color[] Mix(RelativeSaturationLevels level, ReadOnlySpan<Color> a, ReadOnlySpan<Color> b)
        => Mix(Mix, level, a, b);

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="merge">The handler to merge each channel.</param>
    /// <param name="a">The blend color.</param>
    /// <param name="b">The base color.</param>
    /// <returns>A new color mixed.</returns>
    public static Color[] Mix(Func<byte, byte, ColorChannels, byte> merge, ReadOnlySpan<Color> a, ReadOnlySpan<Color> b)
        => Mix(Mix, merge, a, b);

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="type">The type to mix colors.</param>
    /// <param name="a">The collection of blend color.</param>
    /// <param name="b">The collection of base color.</param>
    /// <returns>A new color collection mixed.</returns>
    public static Color[] Mix(ColorMixTypes type, Color[] a, Color[] b)
        => Mix(Mix, type, a, b);

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="level">The relative saturation level.</param>
    /// <param name="a">The collection of blend color.</param>
    /// <param name="b">The collection of base color.</param>
    /// <returns>A new color collection mixed.</returns>
    public static Color[] Mix(RelativeSaturationLevels level, Color[] a, Color[] b)
        => Mix(Mix, level, a, b);

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="merge">The handler to merge each channel.</param>
    /// <param name="a">The blend color.</param>
    /// <param name="b">The base color.</param>
    /// <returns>A new color mixed.</returns>
    public static Color[] Mix(Func<byte, byte, ColorChannels, byte> merge, Color[] a, Color[] b)
        => Mix(Mix, merge, a, b);

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="merge">The handler to merge each color.</param>
    /// <param name="kind">The way or options to merge.</param>
    /// <param name="a">The collection of blend color.</param>
    /// <param name="b">The collection of base color.</param>
    /// <returns>A new color collection mixed.</returns>
    private static IEnumerable<Color> Mix<T>(Func<T, Color, Color, Color> merge, T kind, IEnumerable<Color> a, IEnumerable<Color> b)
    {
        var arr = b.ToList();
        var i = -1;
        foreach (var item in a)
        {
            i++;
            if (i < arr.Count)
                yield return merge(kind, item, arr[i]);
            else
                yield return item;
        }

        i++;
        for (; i < arr.Count; i++)
        {
            yield return arr[i];
        }
    }

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="merge">The handler to merge each color.</param>
    /// <param name="kind">The way or options to merge.</param>
    /// <param name="a">The collection of blend color.</param>
    /// <param name="b">The collection of base color.</param>
    /// <returns>A new color collection mixed.</returns>
    private static Color[] Mix<T>(Func<T, Color, Color, Color> merge, T kind, ReadOnlySpan<Color> a, ReadOnlySpan<Color> b)
    {
        var i = -1;
        var arr = new Color[Math.Max(a.Length, b.Length)];
        foreach (var item in a)
        {
            i++;
            if (i < b.Length)
                arr[i] = merge(kind, item, b[i]);
            else
                arr[i] = item;
        }

        i++;
        for (; i < b.Length; i++)
        {
            arr[i] = arr[i];
        }

        return arr;
    }

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="merge">The handler to merge each color.</param>
    /// <param name="kind">The way or options to merge.</param>
    /// <param name="a">The collection of blend color.</param>
    /// <param name="b">The collection of base color.</param>
    /// <returns>A new color collection mixed.</returns>
    private static Color[] Mix<T>(Func<T, Color, Color, Color> merge, T kind, Color[] a, ReadOnlySpan<Color> b)
    {
        var i = -1;
        var arr = new Color[Math.Max(a.Length, b.Length)];
        foreach (var item in a)
        {
            i++;
            if (i < b.Length)
                arr[i] = merge(kind, item, b[i]);
            else
                arr[i] = item;
        }

        i++;
        for (; i < b.Length; i++)
        {
            arr[i] = arr[i];
        }

        return arr;
    }

#if NETFRAMEWORK
    /// <summary>
    /// Mixes bitmaps.
    /// </summary>
    /// <param name="type">The type to mix bitmaps.</param>
    /// <param name="a">The blend bitmap.</param>
    /// <param name="b">The base bitmap.</param>
    /// <returns>A new bitmap mixed.</returns>
    public static Bitmap Mix(ColorMixTypes type, Bitmap a, Bitmap b)
        => Mix(Mix, type, a, b);

    /// <summary>
    /// Mixes colors.
    /// </summary>
    /// <param name="level">The relative saturation level.</param>
    /// <param name="a">The blend color.</param>
    /// <param name="b">The base color.</param>
    /// <returns>A new color mixed.</returns>
    public static Bitmap Mix(RelativeSaturationLevels level, Bitmap a, Bitmap b)
        => Mix(Mix, level, a, b);

    /// <summary>
    /// Mixes bitmaps.
    /// </summary>
    /// <param name="merge">The handler of color merge.</param>
    /// <param name="args">The arguments of color merge.</param>
    /// <param name="a">The blend bitmap.</param>
    /// <param name="b">The base bitmap.</param>
    /// <returns>A new bitmap mixed.</returns>
    private static Bitmap Mix<T>(Func<T, Color, Color, Color> merge, T args, Bitmap a, Bitmap b)
    {
        if (a == null || b == null) return null;
        var w = Math.Max(a.Width, b.Width);
        var h = Math.Max(a.Height, b.Height);
        var n = new Bitmap(w, h);
        int y;
        for (var x = 0; x < w; x++)
        {
            if (x < a.Width && x < b.Width)
            {
                for (y = 0; y < h; y++)
                {
                    if (y < a.Height && x < b.Height)
                    {
                        var c = a.GetPixel(x, y);
                        var d = b.GetPixel(x, y);
                        n.SetPixel(x, y, merge(args, c, d));
                    }
                    else if (y < a.Height)
                    {
                        for (y = 0; y < h; y++)
                        {
                            n.SetPixel(x, y, a.GetPixel(x, y));
                        }
                    }
                    else if (y < b.Height)
                    {
                        for (y = 0; y < h; y++)
                        {
                            n.SetPixel(x, y, b.GetPixel(x, y));
                        }
                    }
                }
            }
            else if (x < a.Width)
            {
                for (y = 0; y < h; y++)
                {
                    if (y < a.Height)
                        n.SetPixel(x, y, a.GetPixel(x, y));
                    else
                        n.SetPixel(x, y, Color.Transparent);
                }
            }
            else if (x < b.Width)
            {
                for (y = 0; y < h; y++)
                {
                    if (y < b.Height)
                        n.SetPixel(x, y, b.GetPixel(x, y));
                    else
                        n.SetPixel(x, y, Color.Transparent);
                }
            }
        }

        return n;
    }
#endif

    private static Color MixByMean(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
    {
        if (aA == bA)
            return Color.FromArgb(
                aA,
                ToChannel((aR + bR) / 2),
                ToChannel((aG + bG) / 2),
                ToChannel((aB + bB) / 2));
        var topAlpha = aA / 255f;
        var bottomAlpha = bA / 255f;
        var total = topAlpha + bottomAlpha;
        return Color.FromArgb(
            Math.Max(aA, bA),
            ToChannel((aR * topAlpha + bR * bottomAlpha) / total),
            ToChannel((aG * topAlpha + bG * bottomAlpha) / total),
            ToChannel((aB * topAlpha + bB * bottomAlpha) / total));
    }

    private static Color MixByCover(Color a, Color b)
    {
        if (a.A == 0) return b;
        if (b.A == 0 || a.A == 255) return a;
        var ratio = a.A * 1f / 255;
        if (b.A < 255) ratio += (255 - b.A) / 255f * (1 - ratio);
        if (ratio >= 1) return a;
        var negRatio = 1 - ratio;
        return Color.FromArgb(
            Math.Max(a.A, b.A),
            ToChannel(a.R * ratio + b.R * negRatio),
            ToChannel(a.G * ratio + b.G * negRatio),
            ToChannel(a.B * ratio + b.B * negRatio));
    }

    private static Color MixByCover(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
    {
        if (aA == 0) return Color.FromArgb(bA, bR, bG, bB);
        if (bA == 0 || aA == 255) return Color.FromArgb(aA, aR, aG, aB);
        var ratio = aA * 1f / 255;
        if (bA < 255) ratio += (255 - bA) / 255f * (1 - ratio);
        if (ratio >= 1) return Color.FromArgb(aA, aR, aG, aB);
        var negRatio = 1 - ratio;
        return Color.FromArgb(
            Math.Max(aA, bA),
            ToChannel(aR * ratio + bR * negRatio),
            ToChannel(aG * ratio + bG * negRatio),
            ToChannel(aB * ratio + bB * negRatio));
    }

    private static Color MixByLighten(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
        => MixWithAlpha(
            Math.Max(aR, bR),
            Math.Max(aG, bG),
            Math.Max(aB, bB),
            aA, aR, aG, aB,
            bA, bR, bG, bB);

    private static Color MixByDarken(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
        => MixWithAlpha(
            Math.Min(aR, bR),
            Math.Min(aG, bG),
            Math.Min(aB, bB),
            aA, aR, aG, aB,
            bA, bR, bG, bB);

    private static Color MixByWetness(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
        => MixWithAlpha(
            Math.Abs(128 - aR) >= Math.Abs(128 - bR) ? aR : bR,
            Math.Abs(128 - aG) >= Math.Abs(128 - bG) ? aG : bG,
            Math.Abs(128 - aB) >= Math.Abs(128 - bB) ? aB : bB,
            aA, aR, aG, aB,
            bA, bR, bG, bB);

    private static Color MixByDryness(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
        => MixWithAlpha(
            Math.Abs(128 - aR) <= Math.Abs(128 - bR) ? aR : bR,
            Math.Abs(128 - aG) <= Math.Abs(128 - bG) ? aG : bG,
            Math.Abs(128 - aB) <= Math.Abs(128 - bB) ? aB : bB,
            aA, aR, aG, aB,
            bA, bR, bG, bB);

    private static Color MixByWeaken(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
        => MixWithAlpha(
            255 - (255 - aR) * (255 - bR) / 255f,
            255 - (255 - aG) * (255 - bG) / 255f,
            255 - (255 - aB) * (255 - bB) / 255f,
            aA, aR, aG, aB,
            bA, bR, bG, bB);

    private static Color MixByDeepen(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
        => MixWithAlpha(
            aR * bR / 255f,
            aG * bG / 255f,
            aB * bB / 255f,
            aA, aR, aG, aB,
            bA, bR, bG, bB);

    private static Color MixByEmphasis(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
    {
        var red = (aR > 127 ? 255 - aR : aR) * (bR > 127 ? 255 - bR : bR) / 255f;
        var green = (aG > 127 ? 255 - aG : aG) * (bG > 127 ? 255 - bG : bG) / 255f;
        var blue = (aB > 127 ? 255 - aB : aB) * (bB > 127 ? 255 - bB : bB) / 255f;
        if (aR > 127) red = (bR > 127 ? 255 : aR) - red;
        else red = bR > 127 ? aR + red : red;
        if (aG > 127) green = (bG > 127 ? 255 : aG) - green;
        else green = bG > 127 ? aG + red : green;
        if (aB > 127) blue = (bB > 127 ? 255 : aB) - blue;
        else blue = bB > 127 ? aB + blue : blue;
        return MixWithAlpha(red, green, blue, aA, aR, aG, aB, bA, bR, bG, bB);
    }

    private static Color MixByAccent(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
    {
        if (aA == bA)
            return Color.FromArgb(
                Math.Max(aA, bA),
                ToChannel(aR + bR),
                ToChannel(aG + bG),
                ToChannel(aB + bB));
        float alpha = aA + bA;
        var topAlpha = aA / alpha;
        var bottomAlpha = bA / alpha;
        return Color.FromArgb(
            Math.Max(aA, bA),
                ToChannel(aR * topAlpha + bR * bottomAlpha),
                ToChannel(aG * topAlpha + bG * bottomAlpha),
                ToChannel(aB * topAlpha + bB * bottomAlpha));
    }

    private static Color MixByAdd(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
    {
        if (aA == bA)
            return Color.FromArgb(
                Math.Max(aA, bA),
                ToChannel(aR + bR),
                ToChannel(aG + bG),
                ToChannel(aB + bB));
        float alpha = aA + bA;
        var topAlpha = aA / alpha;
        var bottomAlpha = bA / alpha;
        return Color.FromArgb(
            Math.Max(aA, bA),
                ToChannel(aR * topAlpha + bR * bottomAlpha),
                ToChannel(aG * topAlpha + bG * bottomAlpha),
                ToChannel(aB * topAlpha + bB * bottomAlpha));
    }

    private static Color MixByRemove(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
    {
        var red = aR - bR;
        if (red < 0) red = 0;
        var green = aG - bG;
        if (green < 0) green = 0;
        var blue = aB - bB;
        if (blue < 0) blue = 0;
        return MixWithAlpha(red, green, blue, aA, aR, aG, aB, bA, bR, bG, bB);
    }

    private static Color MixByDiff(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
        => MixWithAlpha(
            Math.Abs(aR - bR),
            Math.Abs(aG - bG),
            Math.Abs(aB - bB),
            aA, aR, aG, aB,
            bA, bR, bG, bB);

    private static Color MixByDistance(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
    {
        var red = aR - bR;
        if (red < 0) red += 256;
        var green = aG - bG;
        if (green < 0) green += 256;
        var blue = aB - bB;
        if (blue < 0) blue += 256;
        return MixWithAlpha(red, green, blue, aA, aR, aG, aB, bA, bR, bG, bB);
    }

    private static Color MixBySymmetry(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
    {
        var red = 2 * bR - aR;
        if (red < 0) red = 0;
        else if (red > 255) red = 255;
        var green = 2 * bG - aG;
        if (green < 0) green = 0;
        else if (green > 255) green = 255;
        var blue = 2 * bB - aB;
        if (blue < 0) blue = 0;
        else if (blue > 255) blue = 255;
        return MixWithAlpha(red, green, blue, aA, aR, aG, aB, bA, bR, bG, bB);
    }

    private static Color MixByStrengthen(byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
    {
        var red = 2 * aR - bR;
        if (red < 0) red = 0;
        else if (red > 255) red = 255;
        var green = 2 * aG - bG;
        if (green < 0) green = 0;
        else if (green > 255) green = 255;
        var blue = 2 * aB - bB;
        if (blue < 0) blue = 0;
        else if (blue > 255) blue = 255;
        return MixWithAlpha(red, green, blue, aA, aR, aG, aB, bA, bR, bG, bB);
    }

    private static Color MixWithAlpha(int red, int green, int blue, byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
    {
        if (aA == bA)
            return Color.FromArgb(Math.Max(aA, bA), red, green, blue);
        var ratio = Math.Abs(aA - bA) * 1f / Math.Max(aA, bA);
        var negRation = 1 - ratio;
        var c = aA > bA;
        var cR = c ? aR : bR;
        var cG = c ? aG : bG;
        var cB = c ? aB : bB;
        return Color.FromArgb(
            Math.Max(aA, bA),
            ToChannel(red * negRation + cR * ratio),
            ToChannel(green * negRation + cG * ratio),
            ToChannel(blue * negRation + cB * ratio));
    }

    private static Color MixWithAlpha(float red, float green, float blue, byte aA, byte aR, byte aG, byte aB, byte bA, byte bR, byte bG, byte bB)
    {
        if (aA == bA)
            return Color.FromArgb(Math.Max(aA, bA), ToChannel(red), ToChannel(green), ToChannel(blue));
        var ratio = Math.Abs(aA - bA) * 1f / Math.Max(aA, bA);
        var negRation = 1 - ratio;
        var c = aA > bA;
        var cR = c ? aR : bR;
        var cG = c ? aG : bG;
        var cB = c ? aB : bB;
        return Color.FromArgb(
            Math.Max(aA, bA),
            ToChannel(red * negRation + cR * ratio),
            ToChannel(green * negRation + cG * ratio),
            ToChannel(blue * negRation + cB * ratio));
    }
}
