using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Drawing;

/// <summary>
/// Color calculator.
/// </summary>
public static partial class ColorCalculator
{
    /// <summary>
    /// Calculates to get the color with opacity and a given color.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="alpha">The alpha channel. Value is from 0 to 1.</param>
    /// <returns>A color with new alpha channel value.</returns>
    public static Color Opacity(Color value, double alpha)
        => Color.FromArgb(ToChannel(value.A * alpha), value.R, value.G, value.B);

    /// <summary>
    /// Calculates to get the color with opacity and a given color.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="alpha">The alpha channel. Value is from 0 to 1.</param>
    /// <returns>A color with new alpha channel value.</returns>
    public static Color Opacity(Color value, float alpha)
        => Color.FromArgb(ToChannel(value.A * alpha), value.R, value.G, value.B);

    /// <summary>
    /// Calculates to get the color with opacity and a given color.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="alpha">The alpha channel. Value is from 0 to 255.</param>
    /// <returns>A color with new alpha channel value.</returns>
    public static Color Opacity(Color value, byte alpha)
        => Color.FromArgb(ToChannel(alpha / 255f * value.A), value.R, value.G, value.B);

    /// <summary>
    /// Calculates to get the color with opacity and a given color.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="alpha">The alpha channel. Value is from 0 to 1.</param>
    /// <param name="resetOriginalAlpha">true if use new alpha channel directly instead base the current one; otherwise, false.</param>
    /// <returns>A color with new alpha channel value.</returns>
    public static Color Opacity(Color value, double alpha, bool resetOriginalAlpha)
        => Color.FromArgb(resetOriginalAlpha ? ToChannel(alpha * 255) : ToChannel(value.A * alpha), value.R, value.G, value.B);

    /// <summary>
    /// Calculates to get the color with opacity and a given color.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="alpha">The alpha channel. Value is from 0 to 1.</param>
    /// <param name="resetOriginalAlpha">true if use new alpha channel directly instead base the current one; otherwise, false.</param>
    /// <returns>A color with new alpha channel value.</returns>
    public static Color Opacity(Color value, float alpha, bool resetOriginalAlpha)
        => Color.FromArgb(resetOriginalAlpha ? ToChannel(alpha * 255) : ToChannel(value.A * alpha), value.R, value.G, value.B);

    /// <summary>
    /// Calculates to get the color with opacity and a given color.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="alpha">The alpha channel. Value is from 0 to 255.</param>
    /// <param name="resetOriginalAlpha">true if use new alpha channel directly instead base the current one; otherwise, false.</param>
    /// <returns>A color with new alpha channel value.</returns>
    public static Color Opacity(Color value, byte alpha, bool resetOriginalAlpha)
        => Color.FromArgb(resetOriginalAlpha ? ToChannel(alpha) : ToChannel(alpha / 255f * value.A), value.R, value.G, value.B);

    /// <summary>
    /// Calculates to get the color with opacity and a given color.
    /// </summary>
    /// <param name="value">The source color collection.</param>
    /// <param name="alpha">The alpha channel. Value is from 0 to 255.</param>
    /// <returns>A color collection with new alpha channel value.</returns>
    public static IEnumerable<Color> Opacity(IEnumerable<Color> value, double alpha)
        => value?.Select(ele => Opacity(ele, alpha));

    /// <summary>
    /// Calculates to get the color with opacity and a given color.
    /// </summary>
    /// <param name="value">The source color collection.</param>
    /// <param name="alpha">The alpha channel. Value is from 0 to 255.</param>
    /// <returns>A color collection with new alpha channel value.</returns>
    public static IEnumerable<Color> Opacity(IEnumerable<Color> value, float alpha)
        => value?.Select(ele => Opacity(ele, alpha));

    /// <summary>
    /// Calculates to get the color with opacity and a given color.
    /// </summary>
    /// <param name="value">The source color collection.</param>
    /// <param name="alpha">The alpha channel. Value is from 0 to 255.</param>
    /// <returns>A color collection with new alpha channel value.</returns>
    public static IEnumerable<Color> Opacity(IEnumerable<Color> value, byte alpha)
        => value?.Select(ele => Opacity(ele, alpha));

    /// <summary>
    /// Calculates to get the color with opacity and a given color.
    /// </summary>
    /// <param name="value">The source color collection.</param>
    /// <param name="alpha">The alpha channel. Value is from 0 to 255.</param>
    /// <param name="resetOriginalAlpha">true if use new alpha channel directly instead base the current one; otherwise, false.</param>
    /// <returns>A color collection with new alpha channel value.</returns>
    public static IEnumerable<Color> Opacity(IEnumerable<Color> value, double alpha, bool resetOriginalAlpha)
        => value?.Select(ele => Opacity(ele, alpha, resetOriginalAlpha));

    /// <summary>
    /// Calculates to get the color with opacity and a given color.
    /// </summary>
    /// <param name="value">The source color collection.</param>
    /// <param name="alpha">The alpha channel. Value is from 0 to 255.</param>
    /// <param name="resetOriginalAlpha">true if use new alpha channel directly instead base the current one; otherwise, false.</param>
    /// <returns>A color collection with new alpha channel value.</returns>
    public static IEnumerable<Color> Opacity(IEnumerable<Color> value, float alpha, bool resetOriginalAlpha)
        => value?.Select(ele => Opacity(ele, alpha, resetOriginalAlpha));

    /// <summary>
    /// Calculates to get the color with opacity and a given color.
    /// </summary>
    /// <param name="value">The source color collection.</param>
    /// <param name="alpha">The alpha channel. Value is from 0 to 255.</param>
    /// <param name="resetOriginalAlpha">true if use new alpha channel directly instead base the current one; otherwise, false.</param>
    /// <returns>A color collection with new alpha channel value.</returns>
    public static IEnumerable<Color> Opacity(IEnumerable<Color> value, byte alpha, bool resetOriginalAlpha)
        => value?.Select(ele => Opacity(ele, alpha, resetOriginalAlpha));

    /// <summary>
    /// Creates a new color by set a channel to a given color.
    /// </summary>
    /// <param name="value">The base color.</param>
    /// <param name="channel">The channel to set.</param>
    /// <param name="newValue">The new value of channel.</param>
    /// <returns>A color with new channel value.</returns>
    public static Color WithChannel(Color value, ColorChannels channel, byte newValue)
        => Color.FromArgb(
            channel.HasFlag((ColorChannels)8) ? newValue : value.A,
            channel.HasFlag(ColorChannels.Red) ? newValue : value.R,
            channel.HasFlag(ColorChannels.Green) ? newValue : value.G,
            channel.HasFlag(ColorChannels.Blue) ? newValue : value.B);

    /// <summary>
    /// Gets the channel value of a specific color.
    /// </summary>
    /// <param name="value">The color to get the channel.</param>
    /// <param name="channel">The channel to get.</param>
    /// <returns>The channel value.</returns>
    public static byte GetChannelValue(Color value, ColorChannels channel)
    {
        var arr = new List<byte>();
        if (channel.HasFlag((ColorChannels)8)) arr.Add(value.A);
        if (channel.HasFlag(ColorChannels.Red)) arr.Add(value.R);
        if (channel.HasFlag(ColorChannels.Green)) arr.Add(value.G);
        if (channel.HasFlag(ColorChannels.Blue)) arr.Add(value.B);
        if (arr.Count == 0) return 127;
        float count = 0;
        foreach (var n in arr)
        {
            count += n;
        }

        return (byte)Math.Round(count / arr.Count);
    }

    /// <summary>
    /// Inverts RGB.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <returns>The color to reverse.</returns>
    public static Color Invert(Color value)
        => Color.FromArgb(value.A, 255 - value.R, 255 - value.G, 255 - value.B);

    /// <summary>
    /// Inverts RGB.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="ratio">The ratio to change. Value is from 0 to 1.</param>
    /// <returns>The color to reverse.</returns>
    public static Color Invert(Color value, double ratio)
    {
        if (ratio >= 1) return Color.FromArgb(value.A, 255 - value.R, 255 - value.G, 255 - value.B);
        if (ratio <= 0) return value;
        var a = 255 * ratio;
        var b = 1 - 2 * ratio;
        return Color.FromArgb(value.A,
            ToChannel(a + b * value.R),
            ToChannel(a + b * value.G),
            ToChannel(a + b * value.B));
    }

    /// <summary>
    /// Inverts RGB.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="ratio">The ratio to change. Value is from 0 to 1.</param>
    /// <returns>The color to reverse.</returns>
    public static Color Invert(Color value, float ratio)
    {
        if (ratio >= 1) return Color.FromArgb(value.A, 255 - value.R, 255 - value.G, 255 - value.B);
        if (ratio <= 0) return value;
        var a = 255 * ratio;
        var b = 1 - 2 * ratio;
        return Color.FromArgb(value.A,
            ToChannel(a + b * value.R),
            ToChannel(a + b * value.G),
            ToChannel(a + b * value.B));
    }

    /// <summary>
    /// Inverts RGB.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <returns>The color to reverse.</returns>
    public static IEnumerable<Color> Invert(IEnumerable<Color> value)
        => value?.Select(ele => Invert(ele));

    /// <summary>
    /// Inverts RGB.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The ratio to change. Value is from 0 to 1.</param>
    /// <returns>The color to reverse.</returns>
    public static IEnumerable<Color> Invert(IEnumerable<Color> value, double ratio)
        => value?.Select(ele => Invert(ele, ratio));

    /// <summary>
    /// Inverts RGB.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The ratio to change. Value is from 0 to 1.</param>
    /// <returns>The color to reverse.</returns>
    public static IEnumerable<Color> Invert(IEnumerable<Color> value, float ratio)
        => value?.Select(ele => Invert(ele, ratio));

    /// <summary>
    /// Rotates hue.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="amount">The hue to rotate. Value is from 0 to 360.</param>
    /// <returns>A new color with hue rotation.</returns>
    public static Color RotateHue(Color value, byte amount)
        => RotateHue(value, (float)amount);

    /// <summary>
    /// Rotates hue.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="amount">The hue to rotate. Value is from 0 to 360.</param>
    /// <returns>A new color with hue rotation.</returns>
    public static Color RotateHue(Color value, int amount)
        => RotateHue(value, (float)amount);

    /// <summary>
    /// Rotates hue.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="amount">The hue to rotate. Value is from 0 to 360.</param>
    /// <returns>A new color with hue rotation.</returns>
    public static Color RotateHue(Color value, double amount)
    {
        if (double.IsNaN(amount)) return value;
        if (amount > 360 || amount < 0) amount %= 360;
        if (amount == 0) return value;
        var (h, s, l) = ToHSL(value);
        var hue = h + amount;
        if (hue < 0) hue += 360;
        else if (hue > 360) hue -= 360;
        return FromHSL(hue, s, l);
    }

    /// <summary>
    /// Rotates hue.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="amount">The hue to rotate. Value is from 0 to 360.</param>
    /// <returns>A new color with hue rotation.</returns>
    public static Color RotateHue(Color value, float amount)
    {
        if (float.IsNaN(amount)) return value;
        var hsl = ToSingleHSL(value);
        if (amount > 360 || amount < 0) amount %= 360;
        var hue = hsl.Item1 + amount;
        if (hue < 0) hue += 360;
        else if (hue > 360) hue -= 360;
        return FromHSL(hue, hsl.Item2, hsl.Item3);
    }

    /// <summary>
    /// Rotates hue.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="amount">The hue to rotate. Value is from 0 to 360.</param>
    /// <returns>A new color with hue rotation.</returns>
    public static IEnumerable<Color> RotateHue(IEnumerable<Color> value, byte amount)
        => RotateHue(value, (float)amount);

    /// <summary>
    /// Rotates hue.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="amount">The hue to rotate. Value is from 0 to 360.</param>
    /// <returns>A new color with hue rotation.</returns>
    public static IEnumerable<Color> RotateHue(IEnumerable<Color> value, int amount)
        => RotateHue(value, (float)amount);

    /// <summary>
    /// Rotates hue.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="amount">The hue to rotate. Value is from 0 to 360.</param>
    /// <returns>A new color with hue rotation.</returns>
    public static IEnumerable<Color> RotateHue(IEnumerable<Color> value, double amount)
        => value?.Select(ele => RotateHue(ele, amount));

    /// <summary>
    /// Rotates hue.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="amount">The hue to rotate. Value is from 0 to 360.</param>
    /// <returns>A new color with hue rotation.</returns>
    public static IEnumerable<Color> RotateHue(IEnumerable<Color> value, float amount)
        => value?.Select(ele => RotateHue(ele, amount));

    /// <summary>
    /// Adjusts color balance.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="channel">The channel to set.</param>
    /// <param name="ratio">The ratio to change. Value is from -1 to 1.</param>
    /// <returns>The color after color balance.</returns>
    public static Color ColorBalance(Color value, ColorChannels channel, double ratio)
    {
        if (ratio == 0) return value;
        var white = ratio > 0 ? 255 : 0;
        var black = ratio < 0 ? 255 : 0;
        ratio = Math.Abs(ratio);
        return Color.FromArgb(
            value.A,
            ToChannel(((channel.HasFlag(ColorChannels.Red) ? white : black) - value.R) * ratio + value.R),
            ToChannel(((channel.HasFlag(ColorChannels.Green) ? white : black) - value.G) * ratio + value.G),
            ToChannel(((channel.HasFlag(ColorChannels.Blue) ? white : black) - value.B) * ratio + value.B));
    }

    /// <summary>
    /// Adjusts color balance.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="channel">The channel to set.</param>
    /// <param name="ratio">The ratio to change. Value is from -1 to 1.</param>
    /// <returns>The color after color balance.</returns>
    public static Color ColorBalance(Color value, ColorChannels channel, float ratio)
    {
        if (ratio == 0) return value;
        var white = ratio > 0 ? 255 : 0;
        var black = ratio < 0 ? 255 : 0;
        ratio = Math.Abs(ratio);
        return Color.FromArgb(
            value.A,
            ToChannel(((channel.HasFlag(ColorChannels.Red) ? white : black) - value.R) * ratio + value.R),
            ToChannel(((channel.HasFlag(ColorChannels.Green) ? white : black) - value.G) * ratio + value.G),
            ToChannel(((channel.HasFlag(ColorChannels.Blue) ? white : black) - value.B) * ratio + value.B));
    }

    /// <summary>
    /// Adjusts color balance.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="redRatio">The ratio to change for red channel. Value is from -1 to 1.</param>
    /// <param name="greenRatio">The ratio to change for green channel. Value is from -1 to 1.</param>
    /// <param name="blueRatio">The ratio to change for blue channel. Value is from -1 to 1.</param>
    /// <returns>The color after color balance.</returns>
    public static Color ColorBalance(Color value, double redRatio, double greenRatio, double blueRatio)
    {
        int red = value.R;
        if (redRatio != 0) red = ToChannel(((redRatio > 0 ? 255 : 0) - red) * Math.Abs(redRatio) + red);
        int green = value.G;
        if (greenRatio != 0) green = ToChannel(((greenRatio > 0 ? 255 : 0) - green) * Math.Abs(greenRatio) + green);
        int blue = value.B;
        if (blueRatio != 0) blue = ToChannel(((blueRatio > 0 ? 255 : 0) - blue) * Math.Abs(blueRatio) + blue);
        return Color.FromArgb(value.A, red, green, blue);
    }

    /// <summary>
    /// Adjusts color balance.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="redRatio">The ratio to change for red channel. Value is from -1 to 1.</param>
    /// <param name="greenRatio">The ratio to change for green channel. Value is from -1 to 1.</param>
    /// <param name="blueRatio">The ratio to change for blue channel. Value is from -1 to 1.</param>
    /// <returns>The color after color balance.</returns>
    public static Color ColorBalance(Color value, float redRatio, float greenRatio, float blueRatio)
    {
        int red = value.R;
        if (redRatio != 0) red = ToChannel(((redRatio > 0 ? 255 : 0) - red) * Math.Abs(redRatio) + red);
        int green = value.G;
        if (greenRatio != 0) green = ToChannel(((greenRatio > 0 ? 255 : 0) - green) * Math.Abs(greenRatio) + green);
        int blue = value.B;
        if (blueRatio != 0) blue = ToChannel(((blueRatio > 0 ? 255 : 0) - blue) * Math.Abs(blueRatio) + blue);
        return Color.FromArgb(value.A, red, green, blue);
    }

    /// <summary>
    /// Adjusts color balance.
    /// </summary>
    /// <param name="value">The source color collection.</param>
    /// <param name="channel">The channel to set.</param>
    /// <param name="ratio">The ratio to change. Value is from -1 to 1.</param>
    /// <returns>The color collection after color balance.</returns>
    public static IEnumerable<Color> ColorBalance(IEnumerable<Color> value, ColorChannels channel, double ratio)
        => value?.Select(ele => ColorBalance(ele, channel, ratio));

    /// <summary>
    /// Adjusts color balance.
    /// </summary>
    /// <param name="value">The source color collection.</param>
    /// <param name="channel">The channel to set.</param>
    /// <param name="ratio">The ratio to change. Value is from -1 to 1.</param>
    /// <returns>The color collection after color balance.</returns>
    public static IEnumerable<Color> ColorBalance(IEnumerable<Color> value, ColorChannels channel, float ratio)
        => value?.Select(ele => ColorBalance(ele, channel, ratio));

    /// <summary>
    /// Gets a collection.
    /// </summary>
    /// <param name="from">The color from.</param>
    /// <param name="to">The color to.</param>
    /// <param name="count">The count of color to return.</param>
    /// <returns>A collection of console text.</returns>
    public static IEnumerable<Color> LinearGradient(Color from, Color to, int count)
    {
        if (count < 1) yield break;
        if (count == 1)
        {
            yield return Color.FromArgb((from.A + to.A) / 2, (from.R + to.R) / 2, (from.G + to.G) / 2, (from.B + to.B) / 2);
            yield break;
        }

        yield return from;
        var steps = count - 1;
        var deltaA = (to.A - from.A) * 1f / steps;
        var deltaR = (to.R - from.R) * 1f / steps;
        var deltaG = (to.G - from.B) * 1f / steps;
        var deltaB = (to.B - from.B) * 1f / steps;
        float a = from.A;
        float r = from.R;
        float g = from.G;
        float b = from.B;
        for (var i = 1; i < steps; i++)
        {
            var c = Color.FromArgb(
                PlusChannel(ref a, deltaA),
                PlusChannel(ref r, deltaR),
                PlusChannel(ref g, deltaG),
                PlusChannel(ref b, deltaB));
            yield return c;
        }

        yield return to;
    }

    private static int PlusChannel(ref float c, float delta)
    {
#if NETFRAMEWORK
        var r = (int)Math.Round(c + delta);
#else
        var r = (int)MathF.Round(c + delta);
#endif
        if (r < 0) return 0;
        else if (r > 255) return 255;
        return r;
    }

    private static int ToChannel(float c)
    {
#if NETFRAMEWORK
        var r = (int)Math.Round(c);
#else
        var r = (int)MathF.Round(c);
#endif
        if (r < 0) return 0;
        else if (r > 255) return 255;
        return r;
    }

    private static int ToChannel(double c)
    {
        var r = (int)Math.Round(c);
        if (r < 0) return 0;
        else if (r > 255) return 255;
        return r;
    }
}
